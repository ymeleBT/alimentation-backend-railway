using System.Net;
using System.Security.Claims;
using AlimentationDongmo.Api.Contracts;
using AlimentationDongmo.Api.Services;
using AlimentationDongmo.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlimentationDongmo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.Actif || !await _userManager.CheckPasswordAsync(user, request.MotDePasse))
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

        return Ok(new LoginResponse(token, expiresAt, new UtilisateurDto(user.Id, user.NomComplet, user.Email!, roles)));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UtilisateurDto>> Me()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var user = await _userManager.FindByEmailAsync(email ?? string.Empty);
        if (user is null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UtilisateurDto(user.Id, user.NomComplet, user.Email!, roles));
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<UtilisateurDto>> UpdateMe(MonProfilUpdateDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user is null) return Unauthorized();

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

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UtilisateurDto(user.Id, user.NomComplet, user.Email!, roles));
    }

    [HttpPut("me/mot-de-passe")]
    [Authorize]
    public async Task<IActionResult> ChangerMotDePasse(ChangerMotDePasseDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user is null) return Unauthorized();

        if (!await _userManager.CheckPasswordAsync(user, dto.AncienMotDePasse))
            return BadRequest(new { message = "L'ancien mot de passe est incorrect." });

        var result = await _userManager.ChangePasswordAsync(user, dto.AncienMotDePasse, dto.NouveauMotDePasse);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        return NoContent();
    }

    [HttpPost("mot-de-passe-oublie")]
    [AllowAnonymous]
    public async Task<IActionResult> MotDePasseOublie(MotDePasseOublieDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is not null && user.Actif)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var lien = $"{frontendBaseUrl}/reinitialiser-mot-de-passe?email={WebUtility.UrlEncode(user.Email)}&token={WebUtility.UrlEncode(token)}";

            var html = $"""
                <p>Bonjour {WebUtility.HtmlEncode(user.NomComplet)},</p>
                <p>Vous avez demandé la réinitialisation de votre mot de passe pour Alimentation Dongmo.</p>
                <p><a href="{lien}">Cliquez ici pour réinitialiser votre mot de passe</a></p>
                <p>Si vous n'êtes pas à l'origine de cette demande, ignorez cet email.</p>
                """;

            await _emailSender.SendAsync(user.Email!, "Réinitialisation de votre mot de passe", html);
        }

        // Toujours la même réponse, que l'email existe ou non, pour éviter l'énumération de comptes.
        return Ok(new { message = "Si un compte existe avec cet email, un lien de réinitialisation vient d'être envoyé." });
    }

    [HttpPost("reinitialiser-mot-de-passe")]
    [AllowAnonymous]
    public async Task<IActionResult> ReinitialiserMotDePasse(ReinitialiserMotDePasseDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return BadRequest(new { message = "Ce lien de réinitialisation n'est plus valide." });

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NouveauMotDePasse);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        return NoContent();
    }

    private async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return id is null ? null : await _userManager.FindByIdAsync(id);
    }
}
