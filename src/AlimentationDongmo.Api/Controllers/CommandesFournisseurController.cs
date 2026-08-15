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
[Authorize(Roles = $"{Roles.Administrateur},{Roles.Approvisionnement}")]
public class CommandesFournisseurController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CommandesFournisseurController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CommandeFournisseurDto>>> GetAll(
        [FromQuery] string? statut, [FromQuery] int? fournisseurId, [FromQuery] int page = 1, [FromQuery] int taille = 20)
    {
        var query = _db.CommandesFournisseur
            .Include(c => c.Fournisseur)
            .Include(c => c.Lignes).ThenInclude(l => l.Produit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statut) && Enum.TryParse<StatutCommandeFournisseur>(statut, true, out var s))
            query = query.Where(c => c.Statut == s);
        if (fournisseurId.HasValue) query = query.Where(c => c.FournisseurId == fournisseurId.Value);

        var ordered = query.OrderByDescending(c => c.DateCommande);

        var pageActuelle = page < 1 ? 1 : page;
        var tailleEffective = taille < 1 ? 20 : taille;
        var total = await ordered.CountAsync();
        var commandes = await ordered
            .Skip((pageActuelle - 1) * tailleEffective)
            .Take(tailleEffective)
            .ToListAsync();
        var totalPages = (int)Math.Ceiling(total / (double)tailleEffective);

        return Ok(new PagedResultDto<CommandeFournisseurDto>(commandes.Select(ToDto).ToList(), total, pageActuelle, tailleEffective, totalPages));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommandeFournisseurDto>> GetById(int id)
    {
        var commande = await _db.CommandesFournisseur
            .Include(c => c.Fournisseur)
            .Include(c => c.Lignes).ThenInclude(l => l.Produit)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (commande is null) return NotFound();

        return Ok(ToDto(commande));
    }

    [HttpPost]
    public async Task<ActionResult<CommandeFournisseurDto>> Create(CommandeFournisseurCreateDto dto)
    {
        if (dto.Lignes is null || dto.Lignes.Count == 0)
            return BadRequest(new { message = "La commande doit contenir au moins une ligne." });

        var fournisseur = await _db.Fournisseurs.FindAsync(dto.FournisseurId);
        if (fournisseur is null) return BadRequest(new { message = "Fournisseur introuvable." });

        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var produitIds = dto.Lignes.Select(l => l.ProduitId).Distinct().ToList();
        var produits = await _db.Produits.Where(p => produitIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        var prixFournisseur = await _db.ProduitFournisseurs
            .Where(pf => pf.FournisseurId == dto.FournisseurId && produitIds.Contains(pf.ProduitId))
            .ToDictionaryAsync(pf => pf.ProduitId, pf => pf.PrixAchatFournisseur);

        var commande = new CommandeFournisseur
        {
            FournisseurId = dto.FournisseurId,
            DateCommande = DateTime.UtcNow,
            Statut = StatutCommandeFournisseur.EnAttente,
            CreeParUtilisateurId = userId,
            NumeroCommande = $"CF{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}"
        };

        foreach (var ligne in dto.Lignes)
        {
            if (!produits.TryGetValue(ligne.ProduitId, out var produit))
                return BadRequest(new { message = $"Produit {ligne.ProduitId} introuvable." });
            if (ligne.QuantiteCommandee <= 0)
                return BadRequest(new { message = $"Quantité invalide pour {produit.Nom}." });

            var prixUnitaire = prixFournisseur.TryGetValue(ligne.ProduitId, out var prix) ? prix : produit.PrixAchat;
            commande.Lignes.Add(new LigneCommandeFournisseur
            {
                ProduitId = ligne.ProduitId,
                QuantiteCommandee = ligne.QuantiteCommandee,
                QuantiteRecue = 0,
                PrixUnitaire = prixUnitaire
            });
        }

        _db.CommandesFournisseur.Add(commande);
        await _db.SaveChangesAsync();

        var saved = await _db.CommandesFournisseur
            .Include(c => c.Fournisseur)
            .Include(c => c.Lignes).ThenInclude(l => l.Produit)
            .FirstAsync(c => c.Id == commande.Id);

        return CreatedAtAction(nameof(GetById), new { id = commande.Id }, ToDto(saved));
    }

    [HttpPost("{id:int}/recevoir")]
    public async Task<ActionResult<CommandeFournisseurDto>> Recevoir(int id, ReceptionCommandeCreateDto dto)
    {
        var commande = await _db.CommandesFournisseur
            .Include(c => c.Fournisseur)
            .Include(c => c.Lignes).ThenInclude(l => l.Produit)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (commande is null) return NotFound();

        if (commande.Statut is StatutCommandeFournisseur.Recue or StatutCommandeFournisseur.Annulee)
            return BadRequest(new { message = "Cette commande a déjà été traitée." });

        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var quantitesRecues = dto.Lignes?.ToDictionary(l => l.ProduitId, l => l.QuantiteRecue);

        var toutRecu = true;
        foreach (var ligne in commande.Lignes)
        {
            var qteRecue = quantitesRecues is not null && quantitesRecues.TryGetValue(ligne.ProduitId, out var q)
                ? q
                : ligne.QuantiteCommandee;

            if (qteRecue < 0 || qteRecue > ligne.QuantiteCommandee)
                return BadRequest(new { message = $"Quantité reçue invalide pour {ligne.Produit?.Nom}." });

            if (qteRecue < ligne.QuantiteCommandee) toutRecu = false;
            ligne.QuantiteRecue = qteRecue;

            if (qteRecue > 0)
            {
                var produit = await _db.Produits.FindAsync(ligne.ProduitId);
                produit!.QuantiteEnStock += qteRecue;

                _db.MouvementsStock.Add(new MouvementStock
                {
                    ProduitId = ligne.ProduitId,
                    Type = TypeMouvementStock.Entree,
                    Source = SourceMouvementStock.ReceptionCommande,
                    Quantite = qteRecue,
                    Date = DateTime.UtcNow,
                    UtilisateurId = userId,
                    Reference = commande.NumeroCommande
                });
            }
        }

        commande.DateReception = DateTime.UtcNow;
        commande.Statut = toutRecu ? StatutCommandeFournisseur.Recue : StatutCommandeFournisseur.RecuePartiellement;

        await _db.SaveChangesAsync();
        return Ok(ToDto(commande));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<IActionResult> Delete(int id)
    {
        var commande = await _db.CommandesFournisseur
            .Include(c => c.Lignes)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (commande is null) return NotFound();

        if (commande.Statut is StatutCommandeFournisseur.Recue or StatutCommandeFournisseur.RecuePartiellement)
            return BadRequest(new { message = "Impossible de supprimer une commande déjà reçue (totalement ou partiellement)." });

        _db.CommandesFournisseur.Remove(commande);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static CommandeFournisseurDto ToDto(CommandeFournisseur c) => new(
        c.Id, c.NumeroCommande, c.FournisseurId, c.Fournisseur?.Nom ?? "?",
        c.DateCommande, c.DateReception, c.Statut.ToString(),
        c.Lignes.Select(l => new LigneCommandeDto(l.ProduitId, l.Produit?.Nom ?? "?", l.QuantiteCommandee, l.QuantiteRecue, l.PrixUnitaire)).ToList());
}
