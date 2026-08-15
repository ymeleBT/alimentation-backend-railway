using AlimentationDongmo.Domain.Entities;
using AlimentationDongmo.Domain.Enums;
using AlimentationDongmo.Infrastructure.Identity;
using AlimentationDongmo.Infrastructure.Persistence;
using AlimentationDongmo.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")!;

var services = new ServiceCollection();
services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Warning));
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           .EnableSensitiveDataLogging(false));

services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var sp = scope.ServiceProvider;

var db = sp.GetRequiredService<ApplicationDbContext>();
var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<int>>>();
var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

db.ChangeTracker.AutoDetectChangesEnabled = false;
db.Database.SetCommandTimeout(180);

Console.WriteLine("=== Seed Alimentation Dongmo ===");

var startDate = new DateTime(2022, 1, 1);
var today = new DateTime(2026, 8, 2);
var random = new Random(20260802);

await ClearAllDataAsync(db);

Console.WriteLine("Création des rôles et comptes utilisateurs...");
var userIds = await SeedUsersAsync(roleManager, userManager);

Console.WriteLine("Création des catégories...");
var categorieIds = await SeedCategoriesAsync(db);

Console.WriteLine("Création des produits...");
var (produitIds, produitInfo) = await SeedProduitsAsync(db, categorieIds);

Console.WriteLine("Création des fournisseurs...");
var fournisseurIds = await SeedFournisseursAsync(db);

Console.WriteLine("Association produits <-> fournisseurs...");
var fournisseurProduits = await SeedProduitFournisseurAsync(db, produitIds, fournisseurIds);

Console.WriteLine($"Simulation des opérations quotidiennes du {startDate:d} au {today:d}...");
await SimulateHistoryAsync(db, produitInfo, fournisseurIds, fournisseurProduits, userIds, startDate, today, random);

Console.WriteLine("Terminé.");

// ------------------------------------------------------------------

static async Task ClearAllDataAsync(ApplicationDbContext db)
{
    Console.WriteLine("Nettoyage des tables existantes...");
    await db.Database.OpenConnectionAsync();
    try
    {
        await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
        string[] tables =
        {
            "LignesVente", "Ventes",
            "LignesCommandeFournisseur", "CommandesFournisseur",
            "MouvementsStock",
            "ProduitFournisseurs",
            "Produits", "Categories", "Fournisseurs",
            "UtilisateurRoles", "UtilisateurClaims", "UtilisateurLogins", "UtilisateurTokens",
            "Utilisateurs", "RoleClaims", "Roles"
        };
        foreach (var table in tables)
        {
            string sql = "TRUNCATE TABLE `" + table + "`;";
            await db.Database.ExecuteSqlRawAsync(sql);
        }
        await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
    }
    finally
    {
        await db.Database.CloseConnectionAsync();
    }
}

static async Task<Dictionary<string, int>> SeedUsersAsync(RoleManager<IdentityRole<int>> roleManager, UserManager<ApplicationUser> userManager)
{
    foreach (var role in AlimentationDongmo.Infrastructure.Identity.Roles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int>(role));
    }

    var ids = new Dictionary<string, int>();
    foreach (var u in SeedData.Utilisateurs)
    {
        var user = new ApplicationUser
        {
            UserName = u.UserName,
            Email = u.Email,
            EmailConfirmed = true,
            NomComplet = u.NomComplet,
            Actif = true,
            DateCreation = new DateTime(2022, 1, 1)
        };
        var result = await userManager.CreateAsync(user, SeedData.DefaultPassword);
        if (!result.Succeeded)
            throw new Exception($"Échec création utilisateur {u.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        await userManager.AddToRoleAsync(user, u.Role);
        ids[u.UserName] = user.Id;
    }
    return ids;
}

static async Task<Dictionary<string, int>> SeedCategoriesAsync(ApplicationDbContext db)
{
    var entities = SeedData.Categories.Select(nom => new Categorie { Nom = nom }).ToList();
    db.Categories.AddRange(entities);
    await db.SaveChangesAsync();
    return entities.ToDictionary(c => c.Nom, c => c.Id);
}

static async Task<(Dictionary<string, int> ids, List<ProduitInfo> infos)> SeedProduitsAsync(ApplicationDbContext db, Dictionary<string, int> categorieIds)
{
    var entities = SeedData.Produits.Select(p => new Produit
    {
        Nom = p.Nom,
        CategorieId = categorieIds[p.Categorie],
        Unite = p.Unite,
        PrixAchat = p.PrixAchat,
        PrixVente = p.PrixVente,
        QuantiteEnStock = 0,
        SeuilAlerte = p.SeuilAlerte,
        Actif = true,
        DateCreation = new DateTime(2022, 1, 1)
    }).ToList();
    db.Produits.AddRange(entities);
    await db.SaveChangesAsync();

    var ids = entities.ToDictionary(p => p.Nom, p => p.Id);
    var infos = entities.Select(e =>
    {
        var seed = SeedData.Produits.First(p => p.Nom == e.Nom);
        return new ProduitInfo(e.Id, e.Nom, e.PrixAchat, e.PrixVente, e.SeuilAlerte, seed.Categorie);
    }).ToList();
    return (ids, infos);
}

static async Task<Dictionary<string, int>> SeedFournisseursAsync(ApplicationDbContext db)
{
    var entities = SeedData.Fournisseurs.Select(f => new Fournisseur
    {
        Nom = f.Nom,
        Contact = f.Contact,
        Telephone = f.Telephone,
        Email = f.Email,
        Adresse = f.Adresse,
        Actif = true
    }).ToList();
    db.Fournisseurs.AddRange(entities);
    await db.SaveChangesAsync();
    return entities.ToDictionary(f => f.Nom, f => f.Id);
}

static async Task<Dictionary<int, List<int>>> SeedProduitFournisseurAsync(
    ApplicationDbContext db, Dictionary<string, int> produitIds, Dictionary<string, int> fournisseurIds)
{
    var links = new List<ProduitFournisseur>();
    var fournisseurProduits = new Dictionary<int, List<int>>();

    foreach (var p in SeedData.Produits)
    {
        var produitId = produitIds[p.Nom];
        var fournisseurId = fournisseurIds[p.Fournisseur];
        links.Add(new ProduitFournisseur
        {
            ProduitId = produitId,
            FournisseurId = fournisseurId,
            PrixAchatFournisseur = p.PrixAchat
        });
        if (!fournisseurProduits.TryGetValue(fournisseurId, out var list))
        {
            list = new List<int>();
            fournisseurProduits[fournisseurId] = list;
        }
        list.Add(produitId);
    }

    db.ProduitFournisseurs.AddRange(links);
    await db.SaveChangesAsync();
    return fournisseurProduits;
}

static async Task SimulateHistoryAsync(
    ApplicationDbContext db,
    List<ProduitInfo> produitInfos,
    Dictionary<string, int> fournisseurIds,
    Dictionary<int, List<int>> fournisseurProduits,
    Dictionary<string, int> userIds,
    DateTime startDate,
    DateTime today,
    Random rnd)
{
    var caissierUserIds = SeedData.Utilisateurs.Where(u => u.Role == "Caissier").Select(u => userIds[u.UserName]).ToArray();
    var approUserId = userIds[SeedData.Utilisateurs.First(u => u.Role == "Approvisionnement").UserName];

    var stock = new Dictionary<int, int>();
    var infoById = produitInfos.ToDictionary(p => p.Id);
    var allProduitIds = produitInfos.Select(p => p.Id).ToArray();

    // Pool pondéré : les catégories à forte rotation apparaissent plus souvent dans les ventes
    var weightedPool = new List<int>();
    foreach (var info in produitInfos)
    {
        var weight = info.Categorie switch
        {
            "Boissons" => 4,
            "Céréales & Féculents" => 3,
            "Conserves" => 2,
            "Sucre, Sel & Condiments" => 2,
            "Boulangerie & Pâtisserie" => 3,
            _ => 1
        };
        for (var i = 0; i < weight; i++) weightedPool.Add(info.Id);
    }

    long ticketCounter = 0;
    var commandeCounter = 0;

    var mouvements = new List<MouvementStock>();
    var ventesBuffer = new List<Vente>();
    var pendingLignesVente = new List<(Vente vente, int produitId, int qte, decimal prixUnitaire, decimal sousTotal)>();
    var commandesBuffer = new List<CommandeFournisseur>();
    var pendingLignesCommande = new List<(CommandeFournisseur commande, int produitId, int qteCommandee, int qteRecue, decimal prixUnitaire)>();

    // Stock de démarrage
    foreach (var info in produitInfos)
    {
        var qte = info.SeuilAlerte * (3 + rnd.Next(0, 4));
        stock[info.Id] = qte;
        mouvements.Add(new MouvementStock
        {
            ProduitId = info.Id,
            Type = TypeMouvementStock.Entree,
            Source = SourceMouvementStock.InventaireInitial,
            Quantite = qte,
            Date = startDate.AddHours(8),
            UtilisateurId = approUserId,
            Reference = "STOCK-INITIAL",
            Commentaire = "Stock de démarrage"
        });
    }

    var nextOrderDate = new Dictionary<int, DateTime>();
    foreach (var fid in fournisseurIds.Values)
        nextOrderDate[fid] = startDate.AddDays(rnd.Next(0, 21));

    var pendingReceptions = new List<(DateTime date, CommandeFournisseur commande, List<(int produitId, int qte)> lignes, bool partial)>();

    var totalDays = (today - startDate).Days + 1;
    var dayCount = 0;

    for (var day = startDate; day <= today; day = day.AddDays(1))
    {
        dayCount++;
        if (dayCount % 200 == 0)
            Console.WriteLine($"  ... jour {dayCount}/{totalDays} ({day:yyyy-MM-dd})");

        // 1) Réceptions de commandes prévues aujourd'hui
        var dueReceptions = pendingReceptions.Where(r => r.date.Date == day.Date).ToList();
        foreach (var rec in dueReceptions)
        {
            foreach (var (produitId, qte) in rec.lignes)
            {
                stock[produitId] += qte;
                mouvements.Add(new MouvementStock
                {
                    ProduitId = produitId,
                    Type = TypeMouvementStock.Entree,
                    Source = SourceMouvementStock.ReceptionCommande,
                    Quantite = qte,
                    Date = rec.date.AddHours(9),
                    UtilisateurId = approUserId,
                    Reference = rec.commande.NumeroCommande,
                    Commentaire = rec.partial ? "Réception partielle" : "Réception commande fournisseur"
                });
            }
            rec.commande.DateReception = rec.date;
            rec.commande.Statut = rec.partial ? StatutCommandeFournisseur.RecuePartiellement : StatutCommandeFournisseur.Recue;
            pendingReceptions.Remove(rec);
        }

        // 2) Commandes fournisseurs à passer aujourd'hui
        foreach (var fid in fournisseurIds.Values)
        {
            if (day.Date < nextOrderDate[fid].Date) continue;

            var catalogue = fournisseurProduits[fid];
            var basStock = catalogue.Where(pid => stock[pid] <= infoById[pid].SeuilAlerte * 2).ToList();
            var choix = basStock.Count >= 2 ? basStock : catalogue.OrderBy(_ => rnd.Next()).Take(Math.Min(4, catalogue.Count)).ToList();
            if (choix.Count == 0) { nextOrderDate[fid] = day.AddDays(rnd.Next(18, 36)); continue; }

            commandeCounter++;
            var commande = new CommandeFournisseur
            {
                FournisseurId = fid,
                DateCommande = day,
                Statut = StatutCommandeFournisseur.EnAttente,
                CreeParUtilisateurId = approUserId,
                NumeroCommande = $"CF{commandeCounter:D5}"
            };
            commandesBuffer.Add(commande);

            var lignesRecues = new List<(int produitId, int qte)>();
            var estPartielle = rnd.NextDouble() < 0.05;
            foreach (var pid in choix)
            {
                var info = infoById[pid];
                var cible = info.SeuilAlerte * 5;
                var qteACommander = Math.Max(cible - stock[pid], info.SeuilAlerte * 2);
                qteACommander = (int)Math.Round(qteACommander * (0.85 + rnd.NextDouble() * 0.3));
                if (qteACommander < 1) qteACommander = info.SeuilAlerte;

                pendingLignesCommande.Add((commande, pid, qteACommander, 0, info.PrixAchat));

                var qteRecue = estPartielle ? (int)Math.Round(qteACommander * (0.75 + rnd.NextDouble() * 0.2)) : qteACommander;
                lignesRecues.Add((pid, qteRecue));
            }

            var receptionDate = day.AddDays(rnd.Next(2, 6));
            if (receptionDate <= today)
                pendingReceptions.Add((receptionDate, commande, lignesRecues, estPartielle));

            nextOrderDate[fid] = day.AddDays(rnd.Next(18, 36));
        }

        // 3) Ventes du jour
        var dow = day.DayOfWeek;
        var nbVentes = dow switch
        {
            DayOfWeek.Sunday => rnd.Next(4, 11),
            DayOfWeek.Saturday => rnd.Next(16, 29),
            _ => rnd.Next(10, 19)
        };

        for (var v = 0; v < nbVentes; v++)
        {
            var nbLignes = rnd.Next(1, 6);
            var lignesVente = new List<(int produitId, int qte, decimal prixUnitaire, decimal sousTotal)>();
            var dejaChoisis = new HashSet<int>();

            for (var l = 0; l < nbLignes; l++)
            {
                var produitId = -1;
                for (var essai = 0; essai < 5; essai++)
                {
                    var candidat = weightedPool[rnd.Next(weightedPool.Count)];
                    if (dejaChoisis.Contains(candidat)) continue;
                    if (stock[candidat] <= 0) continue;
                    produitId = candidat;
                    break;
                }
                if (produitId == -1) continue;

                var info = infoById[produitId];
                var qteSouhaitee = rnd.Next(1, 4);
                var qte = Math.Min(qteSouhaitee, stock[produitId]);
                if (qte <= 0) continue;

                stock[produitId] -= qte;
                dejaChoisis.Add(produitId);
                var sousTotal = qte * info.PrixVente;
                lignesVente.Add((produitId, qte, info.PrixVente, sousTotal));

                mouvements.Add(new MouvementStock
                {
                    ProduitId = produitId,
                    Type = TypeMouvementStock.Sortie,
                    Source = SourceMouvementStock.Vente,
                    Quantite = qte,
                    Date = day.AddHours(7.5 + rnd.NextDouble() * 12),
                    UtilisateurId = caissierUserIds[rnd.Next(caissierUserIds.Length)],
                    Reference = null,
                    Commentaire = null
                });
            }

            if (lignesVente.Count == 0) continue;

            ticketCounter++;
            var heure = day.Date.AddHours(7.5 + rnd.NextDouble() * 12);
            var vente = new Vente
            {
                DateHeure = heure,
                CaissierId = caissierUserIds[rnd.Next(caissierUserIds.Length)],
                MontantTotal = lignesVente.Sum(x => x.sousTotal),
                ModePaiement = ModePaiement.Especes,
                Statut = StatutVente.Validee,
                NumeroTicket = $"V{ticketCounter:D7}"
            };
            ventesBuffer.Add(vente);
            foreach (var (produitId, qte, prixUnitaire, sousTotal) in lignesVente)
                pendingLignesVente.Add((vente, produitId, qte, prixUnitaire, sousTotal));
        }

        // 4) Ajustements d'inventaire occasionnels (démarque, casse, correction)
        if (rnd.NextDouble() < 0.6)
        {
            var nbAjustements = rnd.Next(1, 3);
            for (var a = 0; a < nbAjustements; a++)
            {
                var pid = allProduitIds[rnd.Next(allProduitIds.Length)];
                var perte = rnd.NextDouble() < 0.7;
                var delta = perte ? -rnd.Next(1, 4) : rnd.Next(1, 3);
                if (stock[pid] + delta < 0) continue;
                stock[pid] += delta;
                mouvements.Add(new MouvementStock
                {
                    ProduitId = pid,
                    Type = TypeMouvementStock.Ajustement,
                    Source = perte ? SourceMouvementStock.Perte : SourceMouvementStock.CorrectionInventaire,
                    Quantite = delta,
                    Date = day.AddHours(19),
                    UtilisateurId = approUserId,
                    Reference = null,
                    Commentaire = perte ? "Démarque / casse" : "Correction d'inventaire"
                });
            }
        }

        // Flush périodique pour limiter la mémoire et la taille des transactions
        if (dayCount % 45 == 0 || day == today)
        {
            await FlushAsync(db, ventesBuffer, pendingLignesVente, commandesBuffer, pendingLignesCommande, mouvements);
        }
    }

    // Mise à jour finale du stock sur chaque produit
    foreach (var info in produitInfos)
    {
        await db.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Produits SET QuantiteEnStock = {stock[info.Id]} WHERE Id = {info.Id}");
    }
}

static async Task FlushAsync(
    ApplicationDbContext db,
    List<Vente> ventesBuffer,
    List<(Vente vente, int produitId, int qte, decimal prixUnitaire, decimal sousTotal)> pendingLignesVente,
    List<CommandeFournisseur> commandesBuffer,
    List<(CommandeFournisseur commande, int produitId, int qteCommandee, int qteRecue, decimal prixUnitaire)> pendingLignesCommande,
    List<MouvementStock> mouvements)
{
    if (ventesBuffer.Count > 0)
    {
        db.Ventes.AddRange(ventesBuffer);
        await db.SaveChangesAsync();

        var lignes = pendingLignesVente.Select(x => new LigneVente
        {
            VenteId = x.vente.Id,
            ProduitId = x.produitId,
            Quantite = x.qte,
            PrixUnitaire = x.prixUnitaire,
            SousTotal = x.sousTotal
        }).ToList();
        db.LignesVente.AddRange(lignes);
        await db.SaveChangesAsync();

        ventesBuffer.Clear();
        pendingLignesVente.Clear();
    }

    if (commandesBuffer.Count > 0)
    {
        db.CommandesFournisseur.AddRange(commandesBuffer);
        await db.SaveChangesAsync();

        var lignes = pendingLignesCommande.Select(x => new LigneCommandeFournisseur
        {
            CommandeId = x.commande.Id,
            ProduitId = x.produitId,
            QuantiteCommandee = x.qteCommandee,
            QuantiteRecue = x.qteRecue,
            PrixUnitaire = x.prixUnitaire
        }).ToList();
        db.LignesCommandeFournisseur.AddRange(lignes);
        await db.SaveChangesAsync();

        commandesBuffer.Clear();
        pendingLignesCommande.Clear();
    }

    if (mouvements.Count > 0)
    {
        db.MouvementsStock.AddRange(mouvements);
        await db.SaveChangesAsync();
        mouvements.Clear();
    }

    db.ChangeTracker.Clear();
}

record ProduitInfo(int Id, string Nom, decimal PrixAchat, decimal PrixVente, int SeuilAlerte, string Categorie);
