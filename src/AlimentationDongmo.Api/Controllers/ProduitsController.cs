using AlimentationDongmo.Api.Contracts;
using AlimentationDongmo.Domain.Entities;
using AlimentationDongmo.Infrastructure.Identity;
using AlimentationDongmo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlimentationDongmo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProduitsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ProduitsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categorieId,
        [FromQuery] string? recherche,
        [FromQuery] bool? sousLeSeuil,
        [FromQuery] bool inclureInactifs = false,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        var query = _db.Produits.Include(p => p.Categorie).AsQueryable();

        if (!inclureInactifs) query = query.Where(p => p.Actif);
        if (categorieId.HasValue) query = query.Where(p => p.CategorieId == categorieId.Value);
        if (!string.IsNullOrWhiteSpace(recherche)) query = query.Where(p => p.Nom.Contains(recherche));
        if (sousLeSeuil == true) query = query.Where(p => p.QuantiteEnStock <= p.SeuilAlerte);

        var ordered = query
            .OrderBy(p => p.Nom)
            .Select(p => new ProduitDto(
                p.Id, p.Nom, p.CategorieId, p.Categorie!.Nom, p.Unite,
                p.PrixAchat, p.PrixVente, p.QuantiteEnStock, p.SeuilAlerte,
                p.CodeBarre, p.Actif, p.QuantiteEnStock <= p.SeuilAlerte));

        if (!page.HasValue) return Ok(await ordered.ToListAsync());

        var pageActuelle = page.Value < 1 ? 1 : page.Value;
        var tailleEffective = pageSize is > 0 ? pageSize.Value : 20;
        var total = await ordered.CountAsync();
        var items = await ordered.Skip((pageActuelle - 1) * tailleEffective).Take(tailleEffective).ToListAsync();
        var totalPages = (int)Math.Ceiling(total / (double)tailleEffective);

        return Ok(new PagedResultDto<ProduitDto>(items, total, pageActuelle, tailleEffective, totalPages));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProduitDto>> GetById(int id)
    {
        var p = await _db.Produits.Include(x => x.Categorie).FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        return Ok(ToDto(p));
    }

    private static ProduitDto ToDto(Produit p) => new(
        p.Id, p.Nom, p.CategorieId, p.Categorie!.Nom, p.Unite,
        p.PrixAchat, p.PrixVente, p.QuantiteEnStock, p.SeuilAlerte,
        p.CodeBarre, p.Actif, p.QuantiteEnStock <= p.SeuilAlerte);

    [HttpPost]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<ActionResult<ProduitDto>> Create(ProduitUpsertDto dto)
    {
        if (!await _db.Categories.AnyAsync(c => c.Id == dto.CategorieId))
            return BadRequest(new { message = "Catégorie introuvable." });

        var produit = new Produit
        {
            Nom = dto.Nom,
            CategorieId = dto.CategorieId,
            Unite = dto.Unite,
            PrixAchat = dto.PrixAchat,
            PrixVente = dto.PrixVente,
            SeuilAlerte = dto.SeuilAlerte,
            CodeBarre = dto.CodeBarre,
            Actif = dto.Actif,
            QuantiteEnStock = 0,
            DateCreation = DateTime.UtcNow
        };
        _db.Produits.Add(produit);
        await _db.SaveChangesAsync();
        produit.Categorie = await _db.Categories.FindAsync(produit.CategorieId);

        return CreatedAtAction(nameof(GetById), new { id = produit.Id }, ToDto(produit));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<IActionResult> Update(int id, ProduitUpsertDto dto)
    {
        var produit = await _db.Produits.FindAsync(id);
        if (produit is null) return NotFound();

        if (!await _db.Categories.AnyAsync(c => c.Id == dto.CategorieId))
            return BadRequest(new { message = "Catégorie introuvable." });

        produit.Nom = dto.Nom;
        produit.CategorieId = dto.CategorieId;
        produit.Unite = dto.Unite;
        produit.PrixAchat = dto.PrixAchat;
        produit.PrixVente = dto.PrixVente;
        produit.SeuilAlerte = dto.SeuilAlerte;
        produit.CodeBarre = dto.CodeBarre;
        produit.Actif = dto.Actif;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<IActionResult> Deactivate(int id)
    {
        var produit = await _db.Produits.FindAsync(id);
        if (produit is null) return NotFound();

        produit.Actif = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}/definitivement")]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<IActionResult> Delete(int id)
    {
        var produit = await _db.Produits.FindAsync(id);
        if (produit is null) return NotFound();

        _db.Produits.Remove(produit);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Ce produit ne peut pas être supprimé car il est lié à des ventes, réceptions ou mouvements de stock. Vous pouvez le désactiver à la place."
            });
        }

        return NoContent();
    }
}
