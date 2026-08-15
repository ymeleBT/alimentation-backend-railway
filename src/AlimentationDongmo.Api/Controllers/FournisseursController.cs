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
[Authorize(Roles = $"{Roles.Administrateur},{Roles.Approvisionnement}")]
public class FournisseursController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public FournisseursController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FournisseurDto>>> GetAll([FromQuery] bool inclureInactifs = false)
    {
        var query = _db.Fournisseurs.AsQueryable();
        if (!inclureInactifs) query = query.Where(f => f.Actif);

        var fournisseurs = await query.OrderBy(f => f.Nom).Select(f => ToDto(f)).ToListAsync();
        return Ok(fournisseurs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FournisseurDto>> GetById(int id)
    {
        var f = await _db.Fournisseurs.FindAsync(id);
        if (f is null) return NotFound();
        return Ok(ToDto(f));
    }

    [HttpGet("{id:int}/produits")]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduits(int id)
    {
        if (!await _db.Fournisseurs.AnyAsync(f => f.Id == id)) return NotFound();

        var produits = await _db.ProduitFournisseurs
            .Where(pf => pf.FournisseurId == id)
            .Include(pf => pf.Produit).ThenInclude(p => p!.Categorie)
            .Select(pf => pf.Produit!)
            .OrderBy(p => p.Nom)
            .Select(p => new ProduitDto(
                p.Id, p.Nom, p.CategorieId, p.Categorie!.Nom, p.Unite,
                p.PrixAchat, p.PrixVente, p.QuantiteEnStock, p.SeuilAlerte,
                p.CodeBarre, p.Actif, p.QuantiteEnStock <= p.SeuilAlerte))
            .ToListAsync();

        return Ok(produits);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<ActionResult<FournisseurDto>> Create(FournisseurUpsertDto dto)
    {
        var fournisseur = new Fournisseur
        {
            Nom = dto.Nom,
            Contact = dto.Contact,
            Telephone = dto.Telephone,
            Email = dto.Email,
            Adresse = dto.Adresse,
            Actif = dto.Actif
        };
        _db.Fournisseurs.Add(fournisseur);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = fournisseur.Id }, ToDto(fournisseur));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<IActionResult> Update(int id, FournisseurUpsertDto dto)
    {
        var fournisseur = await _db.Fournisseurs.FindAsync(id);
        if (fournisseur is null) return NotFound();

        fournisseur.Nom = dto.Nom;
        fournisseur.Contact = dto.Contact;
        fournisseur.Telephone = dto.Telephone;
        fournisseur.Email = dto.Email;
        fournisseur.Adresse = dto.Adresse;
        fournisseur.Actif = dto.Actif;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrateur)]
    public async Task<IActionResult> Deactivate(int id)
    {
        var fournisseur = await _db.Fournisseurs.FindAsync(id);
        if (fournisseur is null) return NotFound();

        fournisseur.Actif = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static FournisseurDto ToDto(Fournisseur f) => new(f.Id, f.Nom, f.Contact, f.Telephone, f.Email, f.Adresse, f.Actif);
}
