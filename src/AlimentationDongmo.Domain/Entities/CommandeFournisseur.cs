using AlimentationDongmo.Domain.Enums;

namespace AlimentationDongmo.Domain.Entities;

public class CommandeFournisseur
{
    public int Id { get; set; }
    public int FournisseurId { get; set; }
    public Fournisseur? Fournisseur { get; set; }
    public DateTime DateCommande { get; set; }
    public DateTime? DateReception { get; set; }
    public StatutCommandeFournisseur Statut { get; set; } = StatutCommandeFournisseur.EnAttente;
    public int CreeParUtilisateurId { get; set; }
    public string NumeroCommande { get; set; } = string.Empty;

    public ICollection<LigneCommandeFournisseur> Lignes { get; set; } = new List<LigneCommandeFournisseur>();
}
