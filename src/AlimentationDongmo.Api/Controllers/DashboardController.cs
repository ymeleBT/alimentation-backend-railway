using AlimentationDongmo.Api.Contracts;
using AlimentationDongmo.Domain.Enums;
using AlimentationDongmo.Infrastructure.Identity;
using AlimentationDongmo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlimentationDongmo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Administrateur)]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("resume")]
    public async Task<ActionResult<ResumeDashboardDto>> Resume()
    {
        var maintenant = DateTime.UtcNow;
        var aujourdHui = maintenant.Date;

        var ventesValidees = _db.Ventes.Where(v => v.Statut == StatutVente.Validee);

        var caAujourdHui = await ventesValidees
            .Where(v => v.DateHeure.Date == aujourdHui)
            .SumAsync(v => (decimal?)v.MontantTotal) ?? 0;
        var ventesAujourdHui = await ventesValidees.CountAsync(v => v.DateHeure.Date == aujourdHui);

        var caMois = await ventesValidees
            .Where(v => v.DateHeure.Year == maintenant.Year && v.DateHeure.Month == maintenant.Month)
            .SumAsync(v => (decimal?)v.MontantTotal) ?? 0;
        var ventesMois = await ventesValidees
            .CountAsync(v => v.DateHeure.Year == maintenant.Year && v.DateHeure.Month == maintenant.Month);

        var valeurStock = await _db.Produits
            .Where(p => p.Actif)
            .SumAsync(p => (decimal?)(p.QuantiteEnStock * p.PrixAchat)) ?? 0;
        var produitsSousLeSeuil = await _db.Produits.CountAsync(p => p.Actif && p.QuantiteEnStock <= p.SeuilAlerte);

        var commandesEnAttente = await _db.CommandesFournisseur
            .CountAsync(c => c.Statut == StatutCommandeFournisseur.EnAttente);

        return Ok(new ResumeDashboardDto(
            caAujourdHui, ventesAujourdHui, caMois, ventesMois,
            valeurStock, produitsSousLeSeuil, commandesEnAttente));
    }

    [HttpGet("ventes-periode")]
    public async Task<ActionResult<IEnumerable<PointVentePeriodeDto>>> VentesPeriode(
        [FromQuery] DateTime? debut, [FromQuery] DateTime? fin)
    {
        var finPeriode = (fin ?? DateTime.UtcNow).Date;
        var debutPeriode = (debut ?? finPeriode.AddDays(-29)).Date;

        var brut = await _db.Ventes
            .Where(v => v.Statut == StatutVente.Validee && v.DateHeure.Date >= debutPeriode && v.DateHeure.Date <= finPeriode)
            .GroupBy(v => v.DateHeure.Date)
            .Select(g => new { Date = g.Key, Ca = g.Sum(v => v.MontantTotal), Nombre = g.Count() })
            .OrderBy(p => p.Date)
            .ToListAsync();

        return Ok(brut.Select(p => new PointVentePeriodeDto(p.Date, p.Ca, p.Nombre)));
    }

    [HttpGet("top-produits")]
    public async Task<ActionResult<IEnumerable<TopProduitDto>>> TopProduits(
        [FromQuery] DateTime? debut, [FromQuery] DateTime? fin, [FromQuery] int limite = 10)
    {
        var finPeriode = (fin ?? DateTime.UtcNow).Date;
        var debutPeriode = (debut ?? finPeriode.AddDays(-29)).Date;

        var brut = await _db.LignesVente
            .Where(l => l.Vente!.Statut == StatutVente.Validee
                        && l.Vente.DateHeure.Date >= debutPeriode && l.Vente.DateHeure.Date <= finPeriode)
            .GroupBy(l => new { l.ProduitId, l.Produit!.Nom })
            .Select(g => new { g.Key.ProduitId, g.Key.Nom, Quantite = g.Sum(l => l.Quantite), Ca = g.Sum(l => l.SousTotal) })
            .OrderByDescending(p => p.Quantite)
            .Take(limite)
            .ToListAsync();

        return Ok(brut.Select(p => new TopProduitDto(p.ProduitId, p.Nom, p.Quantite, p.Ca)));
    }

    [HttpGet("mouvements-stock")]
    [Authorize(Roles = $"{Roles.Administrateur},{Roles.Approvisionnement}")]
    public async Task<ActionResult<IEnumerable<MouvementStockDto>>> MouvementsStock(
        [FromQuery] int? produitId, [FromQuery] DateTime? debut, [FromQuery] DateTime? fin,
        [FromQuery] int page = 1, [FromQuery] int taille = 50)
    {
        var query = _db.MouvementsStock.Include(m => m.Produit).AsQueryable();

        if (produitId.HasValue) query = query.Where(m => m.ProduitId == produitId.Value);
        if (debut.HasValue) query = query.Where(m => m.Date.Date >= debut.Value.Date);
        if (fin.HasValue) query = query.Where(m => m.Date.Date <= fin.Value.Date);

        var mouvements = await query
            .OrderByDescending(m => m.Date)
            .Skip((page - 1) * taille)
            .Take(taille)
            .Select(m => new MouvementStockDto(
                m.Id, m.ProduitId, m.Produit!.Nom, m.Type.ToString(), m.Source.ToString(),
                m.Quantite, m.Date, m.Reference, m.Commentaire))
            .ToListAsync();

        return Ok(mouvements);
    }
}
