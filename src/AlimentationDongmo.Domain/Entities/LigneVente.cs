namespace AlimentationDongmo.Domain.Entities;

public class LigneVente
{
    public long Id { get; set; }
    public long VenteId { get; set; }
    public Vente? Vente { get; set; }
    public int ProduitId { get; set; }
    public Produit? Produit { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }
}
