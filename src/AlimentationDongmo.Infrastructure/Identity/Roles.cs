namespace AlimentationDongmo.Infrastructure.Identity;

public static class Roles
{
    public const string Administrateur = "Administrateur";
    public const string Caissier = "Caissier";
    public const string Approvisionnement = "Approvisionnement";

    public static readonly string[] All = { Administrateur, Caissier, Approvisionnement };
}
