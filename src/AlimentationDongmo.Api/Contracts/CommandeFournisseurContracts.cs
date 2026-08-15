namespace AlimentationDongmo.Api.Contracts;

public record LigneCommandeCreateDto(int ProduitId, int QuantiteCommandee);

public record CommandeFournisseurCreateDto(int FournisseurId, IList<LigneCommandeCreateDto> Lignes);

public record LigneCommandeDto(int ProduitId, string ProduitNom, int QuantiteCommandee, int QuantiteRecue, decimal PrixUnitaire);

public record CommandeFournisseurDto(
    int Id,
    string NumeroCommande,
    int FournisseurId,
    string FournisseurNom,
    DateTime DateCommande,
    DateTime? DateReception,
    string Statut,
    IList<LigneCommandeDto> Lignes);

public record ReceptionLigneDto(int ProduitId, int QuantiteRecue);

public record ReceptionCommandeCreateDto(IList<ReceptionLigneDto>? Lignes);
