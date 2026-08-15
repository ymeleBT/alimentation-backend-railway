namespace AlimentationDongmo.Seeder;

public record ProduitSeed(
    string Nom,
    string Categorie,
    string Unite,
    decimal PrixAchat,
    decimal PrixVente,
    int SeuilAlerte,
    string Fournisseur);

public record FournisseurSeed(
    string Nom,
    string Contact,
    string Telephone,
    string Email,
    string Adresse);

public record UtilisateurSeed(
    string NomComplet,
    string Email,
    string UserName,
    string Role);

public static class SeedData
{
    public static readonly string[] Categories =
    {
        "Céréales & Féculents",
        "Produits laitiers",
        "Huiles & Matières grasses",
        "Sucre, Sel & Condiments",
        "Boissons",
        "Conserves",
        "Boulangerie & Pâtisserie",
        "Petit-déjeuner",
        "Hygiène & Entretien",
        "Divers"
    };

    public static readonly FournisseurSeed[] Fournisseurs =
    {
        new("SOCAMAC Distribution", "M. Etienne Mballa", "+237 677 12 30 45", "contact@socamac.cm", "Zone industrielle Nkolbisson, Yaoundé"),
        new("Groupe AZUR Alimentaire", "Mme Sylvie Abena", "+237 699 45 67 12", "commercial@azur-alim.cm", "Carrefour Warda, Douala"),
        new("NOVENERGIE Cameroun", "M. Herve Ndzana", "+237 655 33 21 09", "ventes@novenergie.cm", "Zone Industrielle Bassa, Douala"),
        new("SABC Brasseries du Cameroun", "M. Roger Fokou", "+237 233 42 10 10", "distribution@sabc.cm", "Bonabéri, Douala"),
        new("SOSUCAM Sucreries", "Mme Odile Nana", "+237 677 88 22 14", "commercial@sosucam.cm", "Mbandjock, Centre"),
        new("Nestlé Cameroun Distribution", "M. Patrick Essomba", "+237 233 50 12 34", "distribution@nestle-cm.com", "Akwa, Douala"),
        new("Guinness Cameroun SA", "Mme Aicha Oumarou", "+237 233 42 55 66", "commercial@guinness-cm.com", "Bonanjo, Douala"),
        new("Boulangerie Excellence Yaoundé", "M. Christian Belinga", "+237 690 11 22 33", "commandes@excellence-boulangerie.cm", "Mvog-Mbi, Yaoundé"),
        new("Grossiste Mokolo Provisions", "M. Ibrahim Njoya", "+237 674 20 15 88", "mokolo.provisions@gmail.com", "Marché Mokolo, Yaoundé"),
        new("Sanitex Cameroun", "Mme Larissa Tchana", "+237 233 43 20 00", "commercial@sanitex.cm", "Akwa, Douala"),
        new("Fermier Import-Export", "M. Daniel Onana", "+237 677 60 40 20", "import@fermier-ie.cm", "Zone portuaire, Douala"),
        new("Ndokoti Grossiste Alimentaire", "Mme Beatrice Ekwalla", "+237 699 70 18 44", "ndokoti.grossiste@yahoo.fr", "Carrefour Ndokoti, Douala")
    };

    public static readonly UtilisateurSeed[] Utilisateurs =
    {
        new("Alain Dongmo", "alidogmo@dmsacad.com", "admin", "Administrateur"),
        new("Marie Ngo Bell", "marie.ngobell@dmsacad.com", "mngobell", "Caissier"),
        new("Chantal Mballa", "chantal.mballa@dmsacad.com", "cmballa", "Caissier"),
        new("Serge Fotso", "serge.fotso@dmsacad.com", "sfotso", "Caissier"),
        new("Aminatou Bello", "aminatou.bello@dmsacad.com", "abello", "Caissier"),
        new("Bernard Kamga", "bernard.kamga@dmsacad.com", "bkamga", "Approvisionnement"),
        new("Solange Tchoumi", "solange.tchoumi@dmsacad.com", "stchoumi", "Approvisionnement")
    };

    public const string DefaultPassword = "Passer@2026";

    public static readonly ProduitSeed[] Produits =
    {
        // Céréales & Féculents
        new("Riz parfumé importé 25kg", "Céréales & Féculents", "sac", 14000, 16500, 8, "SOCAMAC Distribution"),
        new("Riz brisure importé 25kg", "Céréales & Féculents", "sac", 12500, 14500, 8, "SOCAMAC Distribution"),
        new("Riz local Ndop 25kg", "Céréales & Féculents", "sac", 13000, 15000, 8, "SOCAMAC Distribution"),
        new("Farine de blé 1kg", "Céréales & Féculents", "unité", 700, 900, 25, "Ndokoti Grossiste Alimentaire"),
        new("Farine de maïs 1kg", "Céréales & Féculents", "unité", 600, 800, 25, "Ndokoti Grossiste Alimentaire"),
        new("Semoule de maïs (couscous) 1kg", "Céréales & Féculents", "unité", 650, 850, 25, "Ndokoti Grossiste Alimentaire"),
        new("Spaghetti 500g", "Céréales & Féculents", "unité", 400, 550, 30, "Ndokoti Grossiste Alimentaire"),
        new("Macaroni 500g", "Céréales & Féculents", "unité", 400, 550, 30, "Ndokoti Grossiste Alimentaire"),
        new("Garri 1kg", "Céréales & Féculents", "unité", 500, 700, 25, "Ndokoti Grossiste Alimentaire"),

        // Produits laitiers
        new("Lait en poudre Nido 400g", "Produits laitiers", "unité", 2800, 3300, 20, "Nestlé Cameroun Distribution"),
        new("Lait en poudre Nido 900g", "Produits laitiers", "unité", 5800, 6700, 15, "Nestlé Cameroun Distribution"),
        new("Lait en poudre Nutrimil 400g", "Produits laitiers", "unité", 2400, 2900, 20, "Fermier Import-Export"),
        new("Lait concentré sucré Nestlé 400g", "Produits laitiers", "unité", 900, 1150, 25, "Nestlé Cameroun Distribution"),
        new("Lait concentré Lactel 400g", "Produits laitiers", "unité", 850, 1100, 25, "Fermier Import-Export"),
        new("Yaourt nature 500ml", "Produits laitiers", "unité", 700, 950, 20, "Fermier Import-Export"),
        new("Coffee-Mate 400g", "Produits laitiers", "unité", 2600, 3100, 15, "Nestlé Cameroun Distribution"),

        // Huiles & Matières grasses
        new("Huile végétale Diamaor 1L", "Huiles & Matières grasses", "unité", 1300, 1550, 20, "Fermier Import-Export"),
        new("Huile végétale Mayor 1L", "Huiles & Matières grasses", "unité", 1350, 1600, 20, "Fermier Import-Export"),
        new("Huile végétale Avilo 5L", "Huiles & Matières grasses", "bidon", 6200, 7000, 8, "Fermier Import-Export"),
        new("Huile de palme 1L", "Huiles & Matières grasses", "unité", 1100, 1350, 20, "Groupe AZUR Alimentaire"),
        new("Beurre Président 250g", "Huiles & Matières grasses", "unité", 1800, 2100, 12, "Fermier Import-Export"),
        new("Margarine Planta 250g", "Huiles & Matières grasses", "unité", 900, 1100, 15, "Groupe AZUR Alimentaire"),
        new("Margarine Blue Band 500g", "Huiles & Matières grasses", "unité", 1600, 1900, 15, "Groupe AZUR Alimentaire"),

        // Sucre, Sel & Condiments
        new("Sucre en poudre SOSUCAM 1kg", "Sucre, Sel & Condiments", "unité", 700, 900, 30, "SOSUCAM Sucreries"),
        new("Sel iodé 1kg", "Sucre, Sel & Condiments", "unité", 250, 400, 30, "Grossiste Mokolo Provisions"),
        new("Cube Maggi (boîte de 50)", "Sucre, Sel & Condiments", "boîte", 1500, 1800, 15, "Nestlé Cameroun Distribution"),
        new("Cube Jumbo (boîte de 50)", "Sucre, Sel & Condiments", "boîte", 1400, 1700, 15, "Grossiste Mokolo Provisions"),
        new("Poivre moulu 100g", "Sucre, Sel & Condiments", "unité", 500, 700, 15, "Grossiste Mokolo Provisions"),
        new("Piment moulu 100g", "Sucre, Sel & Condiments", "unité", 400, 600, 15, "Grossiste Mokolo Provisions"),
        new("Vinaigre 1L", "Sucre, Sel & Condiments", "unité", 600, 800, 15, "Groupe AZUR Alimentaire"),
        new("Mayonnaise 500ml", "Sucre, Sel & Condiments", "unité", 1200, 1500, 15, "Groupe AZUR Alimentaire"),

        // Boissons
        new("Coca-Cola 1.5L", "Boissons", "unité", 800, 1000, 40, "NOVENERGIE Cameroun"),
        new("Fanta 1.5L", "Boissons", "unité", 800, 1000, 40, "NOVENERGIE Cameroun"),
        new("Sprite 1.5L", "Boissons", "unité", 800, 1000, 40, "NOVENERGIE Cameroun"),
        new("Top Ananas 1L", "Boissons", "unité", 500, 700, 40, "SABC Brasseries du Cameroun"),
        new("Djino 1L", "Boissons", "unité", 500, 700, 40, "SABC Brasseries du Cameroun"),
        new("Eau minérale Supermont 1.5L", "Boissons", "unité", 350, 500, 50, "NOVENERGIE Cameroun"),
        new("Eau minérale Tangui 1.5L", "Boissons", "unité", 350, 500, 50, "SABC Brasseries du Cameroun"),
        new("Jus Cocktail 1L", "Boissons", "unité", 900, 1150, 25, "NOVENERGIE Cameroun"),
        new("Bière 33 Export 65cl", "Boissons", "unité", 600, 800, 40, "SABC Brasseries du Cameroun"),
        new("Malta Guinness 33cl", "Boissons", "unité", 600, 800, 40, "Guinness Cameroun SA"),

        // Conserves
        new("Sardine Titus (boîte)", "Conserves", "boîte", 500, 650, 30, "Ndokoti Grossiste Alimentaire"),
        new("Sardine Pilchard (boîte)", "Conserves", "boîte", 550, 700, 30, "Ndokoti Grossiste Alimentaire"),
        new("Thon Saupiquet (boîte)", "Conserves", "boîte", 1300, 1600, 20, "Ndokoti Grossiste Alimentaire"),
        new("Corned-beef (boîte)", "Conserves", "boîte", 1500, 1800, 15, "Ndokoti Grossiste Alimentaire"),
        new("Tomate concentrée Cica 400g", "Conserves", "unité", 500, 650, 25, "Groupe AZUR Alimentaire"),
        new("Tomate concentrée Gino 400g", "Conserves", "unité", 500, 650, 25, "Groupe AZUR Alimentaire"),
        new("Tomate concentrée Cabon 70g", "Conserves", "unité", 150, 250, 40, "Groupe AZUR Alimentaire"),

        // Boulangerie & Pâtisserie
        new("Pain de mie 500g", "Boulangerie & Pâtisserie", "unité", 800, 1000, 15, "Boulangerie Excellence Yaoundé"),
        new("Baguette de pain", "Boulangerie & Pâtisserie", "unité", 150, 200, 30, "Boulangerie Excellence Yaoundé"),
        new("Biscuits Ecobis (paquet)", "Boulangerie & Pâtisserie", "paquet", 300, 450, 25, "Boulangerie Excellence Yaoundé"),
        new("Gâteaux secs assortiment", "Boulangerie & Pâtisserie", "paquet", 500, 700, 15, "Boulangerie Excellence Yaoundé"),

        // Petit-déjeuner
        new("Café soluble 100g", "Petit-déjeuner", "unité", 1500, 1850, 15, "Nestlé Cameroun Distribution"),
        new("Thé Lipton (boîte 25 sachets)", "Petit-déjeuner", "boîte", 900, 1150, 15, "Grossiste Mokolo Provisions"),
        new("Milo 400g", "Petit-déjeuner", "unité", 2200, 2650, 20, "Nestlé Cameroun Distribution"),
        new("Ovaltine 400g", "Petit-déjeuner", "unité", 2100, 2550, 15, "Grossiste Mokolo Provisions"),
        new("Confiture 400g", "Petit-déjeuner", "unité", 900, 1150, 15, "Grossiste Mokolo Provisions"),

        // Hygiène & Entretien
        new("Savon de Marseille (pain)", "Hygiène & Entretien", "unité", 350, 500, 25, "Sanitex Cameroun"),
        new("Savon Camy (pain)", "Hygiène & Entretien", "unité", 300, 450, 25, "Sanitex Cameroun"),
        new("Détergent Omo 1kg", "Hygiène & Entretien", "unité", 1400, 1700, 15, "Sanitex Cameroun"),
        new("Détergent Klin 1kg", "Hygiène & Entretien", "unité", 1300, 1600, 15, "Sanitex Cameroun"),
        new("Papier hygiénique (paquet de 4)", "Hygiène & Entretien", "paquet", 800, 1000, 20, "Sanitex Cameroun"),
        new("Dentifrice Signal 100ml", "Hygiène & Entretien", "unité", 700, 900, 20, "Sanitex Cameroun"),

        // Divers
        new("Bonbons assortiment (paquet)", "Divers", "paquet", 500, 700, 20, "Grossiste Mokolo Provisions"),
        new("Chewing-gum (paquet)", "Divers", "paquet", 300, 450, 20, "Grossiste Mokolo Provisions"),
        new("Allumettes (boîte)", "Divers", "boîte", 100, 200, 30, "Grossiste Mokolo Provisions"),
        new("Bougies (paquet de 6)", "Divers", "paquet", 400, 600, 20, "Grossiste Mokolo Provisions"),
        new("Piles AA (paquet de 4)", "Divers", "paquet", 700, 900, 15, "Grossiste Mokolo Provisions")
    };
}
