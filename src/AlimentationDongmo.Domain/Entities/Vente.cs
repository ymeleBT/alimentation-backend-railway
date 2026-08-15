using AlimentationDongmo.Domain.Enums;

namespace AlimentationDongmo.Domain.Entities;

public class Vente
{
    public long Id { get; set; }
    public DateTime DateHeure { get; set; }
    public int CaissierId { get; set; }
    public string? NomClient { get; set; }
    public decimal MontantTotal { get; set; }
    public ModePaiement ModePaiement { get; set; }
    public StatutVente Statut { get; set; } = StatutVente.Validee;
    public string NumeroTicket { get; set; } = string.Empty;

    public ICollection<LigneVente> Lignes { get; set; } = new List<LigneVente>();
}
