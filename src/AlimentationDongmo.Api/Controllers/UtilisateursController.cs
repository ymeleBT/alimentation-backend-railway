using AlimentationDongmo.Api.Contracts;
using AlimentationDongmo.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlimentationDongmo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Administrateur)]
public class UtilisateursController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UtilisateursController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonnelDto>>> GetAll()
    {
        var users = _userManager.Users.OrderBy(u => u.NomComplet).ToList();
        var result = new List<PersonnelDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new PersonnelDto(user.Id, user.NomComplet, user.Email!, user.Actif, roles));
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PersonnelDto>> Create(PersonnelCreateDto dto)
    {
        if (!Roles.All.Contains(dto.Role))
            return BadRequest(new { message = $"Rôle invalide. Rôles autorisés : {string.Join(", ", Roles.All)}." });

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            NomComplet = dto.NomComplet,
            Actif = true,
            DateCreation = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.MotDePasse);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        await _userManager.AddToRoleAsync(user, dto.Role);

        return CreatedAtAction(nameof(GetAll), new { }, new PersonnelDto(user.Id, user.NomComplet, user.Email, user.Actif, new List<string> { dto.Role }));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PersonnelDto>> Update(int id, PersonnelUpdateDto dto)
    {
        if (!Roles.All.Contains(dto.Role))
            return BadRequest(new { message = $"Rôle invalide. Rôles autorisés : {string.Join(", ", Roles.All)}." });

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        user.NomComplet = dto.NomComplet;

        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = await _userManager.SetEmailAsync(user, dto.Email);
            if (!emailResult.Succeeded)
                return BadRequest(new { message = string.Join(" ", emailResult.Errors.Select(e => e.Description)) });

            var userNameResult = await _userManager.SetUserNameAsync(user, dto.Email);
            if (!userNameResult.Succeeded)
                return BadRequest(new { message = string.Join(" ", userNameResult.Errors.Select(e => e.Description)) });

            user.EmailConfirmed = true;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return BadRequest(new { message = string.Join(" ", updateResult.Errors.Select(e => e.Description)) });

        var rolesActuels = await _userManager.GetRolesAsync(user);
        if (!rolesActuels.Contains(dto.Role) || rolesActuels.Count > 1)
        {
            await _userManager.RemoveFromRolesAsync(user, rolesActuels);
            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        if (!string.IsNullOrWhiteSpace(dto.MotDePasse))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.MotDePasse);
            if (!passwordResult.Succeeded)
                return BadRequest(new { message = string.Join(" ", passwordResult.Errors.Select(e => e.Description)) });
        }

        return Ok(new PersonnelDto(user.Id, user.NomComplet, user.Email!, user.Actif, new List<string> { dto.Role }));
    }

    [HttpPut("{id:int}/statut")]
    public async Task<IActionResult> SetStatut(int id, PersonnelStatutDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        user.Actif = dto.Actif;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        return NoContent();
    }
}
