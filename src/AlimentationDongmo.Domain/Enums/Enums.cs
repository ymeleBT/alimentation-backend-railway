namespace AlimentationDongmo.Domain.Enums;

public enum TypeMouvementStock
{
    Entree = 1,
    Sortie = 2,
    Ajustement = 3
}

public enum SourceMouvementStock
{
    Vente = 1,
    ReceptionCommande = 2,
    InventaireInitial = 3,
    CorrectionInventaire = 4,
    Perte = 5
}

public enum ModePaiement
{
    Especes = 1,
    MobileMoney = 2,
    Carte = 3
}

public enum StatutVente
{
    Validee = 1,
    Annulee = 2
}

public enum StatutCommandeFournisseur
{
    EnAttente = 1,
    Recue = 2,
    RecuePartiellement = 3,
    Annulee = 4
}

public enum RoleUtilisateur
{
    Administrateur = 1,
    Caissier = 2,
    Approvisionnement = 3
}
