namespace AlimentationDongmo.Api.Contracts;

public record LigneVenteCreateDto(int ProduitId, int Quantite);

public record VenteCreateDto(IList<LigneVenteCreateDto> Lignes, string ModePaiement, string? NomClient);

public record LigneVenteDto(int ProduitId, string ProduitNom, int Quantite, decimal PrixUnitaire, decimal SousTotal);

public record VenteDto(
    long Id,
    string NumeroTicket,
    DateTime DateHeure,
    string Caissier,
    string? NomClient,
    decimal MontantTotal,
    string ModePaiement,
    string Statut,
    IList<LigneVenteDto> Lignes);
