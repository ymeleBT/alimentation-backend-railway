using AlimentationDongmo.Api.Contracts;
using AlimentationDongmo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlimentationDongmo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CategoriesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Nom)
            .Select(c => new CategorieDto(c.Id, c.Nom))
            .ToListAsync();
        return Ok(categories);
    }
}
