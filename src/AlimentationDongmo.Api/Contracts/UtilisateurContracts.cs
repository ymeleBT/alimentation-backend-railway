namespace AlimentationDongmo.Api.Contracts;

public record PersonnelDto(int Id, string NomComplet, string Email, bool Actif, IList<string> Roles);

public record PersonnelCreateDto(string NomComplet, string Email, string MotDePasse, string Role);

public record PersonnelUpdateDto(string NomComplet, string Email, string Role, string? MotDePasse);

public record PersonnelStatutDto(bool Actif);
