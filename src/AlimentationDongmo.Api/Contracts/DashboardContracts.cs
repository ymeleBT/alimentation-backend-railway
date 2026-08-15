namespace AlimentationDongmo.Api.Contracts;

public record ResumeDashboardDto(
    decimal CaAujourdHui,
    int VentesAujourdHui,
    decimal CaMoisEnCours,
    int VentesMoisEnCours,
    decimal ValeurStock,
    int NombreProduitsSousLeSeuil,
    int CommandesEnAttente);

public record PointVentePeriodeDto(DateTime Date, decimal Ca, int NombreVentes);

public record TopProduitDto(int ProduitId, string ProduitNom, int QuantiteVendue, decimal ChiffreAffaires);

public record MouvementStockDto(
    long Id,
    int ProduitId,
    string ProduitNom,
    string Type,
    string Source,
    int Quantite,
    DateTime Date,
    string? Reference,
    string? Commentaire);
