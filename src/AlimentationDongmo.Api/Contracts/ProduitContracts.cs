namespace AlimentationDongmo.Api.Contracts;

public record CategorieDto(int Id, string Nom);

public record ProduitDto(
    int Id,
    string Nom,
    int CategorieId,
    string CategorieNom,
    string Unite,
    decimal PrixAchat,
    decimal PrixVente,
    int QuantiteEnStock,
    int SeuilAlerte,
    string? CodeBarre,
    bool Actif,
    bool SousLeSeuil);

public record ProduitUpsertDto(
    string Nom,
    int CategorieId,
    string Unite,
    decimal PrixAchat,
    decimal PrixVente,
    int SeuilAlerte,
    string? CodeBarre,
    bool Actif);
