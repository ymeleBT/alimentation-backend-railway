using AlimentationDongmo.Domain.Enums;

namespace AlimentationDongmo.Domain.Entities;

public class MouvementStock
{
    public long Id { get; set; }
    public int ProduitId { get; set; }
    public Produit? Produit { get; set; }
    public TypeMouvementStock Type { get; set; }
    public SourceMouvementStock Source { get; set; }
    public int Quantite { get; set; }
    public DateTime Date { get; set; }
    public int? UtilisateurId { get; set; }
    public string? Reference { get; set; }
    public string? Commentaire { get; set; }
}
