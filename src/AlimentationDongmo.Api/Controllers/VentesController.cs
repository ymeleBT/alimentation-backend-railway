using System.Security.Claims;
using AlimentationDongmo.Api.Contracts;
using AlimentationDongmo.Domain.Entities;
using AlimentationDongmo.Domain.Enums;
using AlimentationDongmo.Infrastructure.Identity;
using AlimentationDongmo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlimentationDongmo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VentesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public VentesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.Administrateur},{Roles.Caissier}")]
    public async Task<ActionResult<IEnumerable<VenteDto>>> GetAll(
        [FromQuery] DateTime? date, [FromQuery] int page = 1, [FromQuery] int taille = 50)
    {
        var query = _db.Ventes
            .Include(v => v.Lignes).ThenInclude(l => l.Produit)
            .AsQueryable();

        if (date.HasValue) query = query.Where(v => v.DateHeure.Date == date.Value.Date);

        var ventes = await query
            .OrderByDescending(v => v.DateHeure)
            .Skip((page - 1) * taille)
            .Take(taille)
            .ToListAsync();

        var caissierNoms = await _db.Users
            .Where(u => ventes.Select(v => v.CaissierId).Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.NomComplet);

        return Ok(ventes.Select(v => ToDto(v, caissierNoms)));
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = $"{Roles.Administrateur},{Roles.Caissier}")]
    public async Task<ActionResult<VenteDto>> GetById(long id)
    {
        var vente = await _db.Ventes.Include(v => v.Lignes).ThenInclude(l => l.Produit)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (vente is null) return NotFound();

        var caissier = await _db.Users.FirstOrDefaultAsync(u => u.Id == vente.CaissierId);
        return Ok(ToDto(vente, new Dictionary<int, string> { [vente.CaissierId] = caissier?.NomComplet ?? "?" }));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Administrateur},{Roles.Caissier}")]
    public async Task<ActionResult<VenteDto>> Create(VenteCreateDto dto)
    {
        if (dto.Lignes is null || dto.Lignes.Count == 0)
            return BadRequest(new { message = "La vente doit contenir au moins une ligne." });

        if (!Enum.TryParse<ModePaiement>(dto.ModePaiement, ignoreCase: true, out var modePaiement))
            return BadRequest(new { message = "Mode de paiement invalide." });

        var caissierIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(caissierIdStr, out var caissierId))
            return Unauthorized();

        var produitIds = dto.Lignes.Select(l => l.ProduitId).Distinct().ToList();
        var produits = await _db.Produits.Where(p => produitIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

        foreach (var ligne in dto.Lignes)
        {
            if (!produits.TryGetValue(ligne.ProduitId, out var produit))
                return BadRequest(new { message = $"Produit {ligne.ProduitId} introuvable." });
            if (ligne.Quantite <= 0)
                return BadRequest(new { message = $"Quantité invalide pour {produit.Nom}." });
            if (produit.QuantiteEnStock < ligne.Quantite)
                return BadRequest(new { message = $"Stock insuffisant pour {produit.Nom} (disponible: {produit.QuantiteEnStock})." });
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();

        var nomClient = string.IsNullOrWhiteSpace(dto.NomClient) ? null : dto.NomClient.Trim();

        var vente = new Vente
        {
            DateHeure = DateTime.UtcNow,
            CaissierId = caissierId,
            NomClient = nomClient,
            ModePaiement = modePaiement,
            Statut = StatutVente.Validee,
            NumeroTicket = $"V{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}"
        };

        decimal total = 0;
        foreach (var ligne in dto.Lignes)
        {
            var produit = produits[ligne.ProduitId];
            var sousTotal = ligne.Quantite * produit.PrixVente;
            total += sousTotal;

            vente.Lignes.Add(new LigneVente
            {
                ProduitId = produit.Id,
                Quantite = ligne.Quantite,
                PrixUnitaire = produit.PrixVente,
                SousTotal = sousTotal
            });

            produit.QuantiteEnStock -= ligne.Quantite;

            _db.MouvementsStock.Add(new MouvementStock
            {
                ProduitId = produit.Id,
                Type = TypeMouvementStock.Sortie,
                Source = SourceMouvementStock.Vente,
                Quantite = ligne.Quantite,
                Date = vente.DateHeure,
                UtilisateurId = caissierId,
                Reference = vente.NumeroTicket
            });
        }
        vente.MontantTotal = total;

        _db.Ventes.Add(vente);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        var caissier = await _db.Users.FirstOrDefaultAsync(u => u.Id == caissierId);
        return CreatedAtAction(nameof(GetById), new { id = vente.Id },
            ToDto(vente, new Dictionary<int, string> { [caissierId] = caissier?.NomComplet ?? "?" }));
    }

    private static VenteDto ToDto(Vente v, IReadOnlyDictionary<int, string> caissierNoms) => new(
        v.Id, v.NumeroTicket, v.DateHeure,
        caissierNoms.TryGetValue(v.CaissierId, out var nom) ? nom : "?",
        v.NomClient,
        v.MontantTotal, v.ModePaiement.ToString(), v.Statut.ToString(),
        v.Lignes.Select(l => new LigneVenteDto(l.ProduitId, l.Produit?.Nom ?? "?", l.Quantite, l.PrixUnitaire, l.SousTotal)).ToList());
}
