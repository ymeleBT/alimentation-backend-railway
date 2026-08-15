namespace AlimentationDongmo.Domain.Entities;

public class LigneCommandeFournisseur
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public CommandeFournisseur? Commande { get; set; }
    public int ProduitId { get; set; }
    public Produit? Produit { get; set; }
    public int QuantiteCommandee { get; set; }
    public int QuantiteRecue { get; set; }
    public decimal PrixUnitaire { get; set; }
}
