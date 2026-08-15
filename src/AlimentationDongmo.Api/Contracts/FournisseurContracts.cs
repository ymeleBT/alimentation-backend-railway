namespace AlimentationDongmo.Api.Contracts;

public record FournisseurDto(int Id, string Nom, string? Contact, string? Telephone, string? Email, string? Adresse, bool Actif);

public record FournisseurUpsertDto(string Nom, string? Contact, string? Telephone, string? Email, string? Adresse, bool Actif);
