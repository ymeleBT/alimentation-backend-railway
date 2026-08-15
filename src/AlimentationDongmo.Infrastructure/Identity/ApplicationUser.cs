using Microsoft.AspNetCore.Identity;

namespace AlimentationDongmo.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string NomComplet { get; set; } = string.Empty;
    public bool Actif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}
