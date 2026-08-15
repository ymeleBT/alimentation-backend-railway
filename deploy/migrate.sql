CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `Categories` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Nom` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Categories` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `Fournisseurs` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Nom` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `Contact` varchar(100) CHARACTER SET utf8mb4 NULL,
        `Telephone` varchar(30) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(150) CHARACTER SET utf8mb4 NULL,
        `Adresse` varchar(250) CHARACTER SET utf8mb4 NULL,
        `Actif` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Fournisseurs` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `Roles` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Roles` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `Utilisateurs` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `NomComplet` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Actif` tinyint(1) NOT NULL,
        `DateCreation` datetime(6) NOT NULL,
        `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
        `EmailConfirmed` tinyint(1) NOT NULL,
        `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
        `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
        `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
        `PhoneNumberConfirmed` tinyint(1) NOT NULL,
        `TwoFactorEnabled` tinyint(1) NOT NULL,
        `LockoutEnd` datetime(6) NULL,
        `LockoutEnabled` tinyint(1) NOT NULL,
        `AccessFailedCount` int NOT NULL,
        CONSTRAINT `PK_Utilisateurs` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `Ventes` (
        `Id` bigint NOT NULL AUTO_INCREMENT,
        `DateHeure` datetime(6) NOT NULL,
        `CaissierId` int NOT NULL,
        `MontantTotal` decimal(12,2) NOT NULL,
        `ModePaiement` int NOT NULL,
        `Statut` int NOT NULL,
        `NumeroTicket` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Ventes` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `Produits` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Nom` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `CategorieId` int NOT NULL,
        `Unite` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `PrixAchat` decimal(12,2) NOT NULL,
        `PrixVente` decimal(12,2) NOT NULL,
        `QuantiteEnStock` int NOT NULL,
        `SeuilAlerte` int NOT NULL,
        `CodeBarre` varchar(50) CHARACTER SET utf8mb4 NULL,
        `ImageUrl` varchar(300) CHARACTER SET utf8mb4 NULL,
        `Actif` tinyint(1) NOT NULL,
        `DateCreation` datetime(6) NOT NULL,
        CONSTRAINT `PK_Produits` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Produits_Categories_CategorieId` FOREIGN KEY (`CategorieId`) REFERENCES `Categories` (`Id`) ON DELETE RESTRICT
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `CommandesFournisseur` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `FournisseurId` int NOT NULL,
        `DateCommande` datetime(6) NOT NULL,
        `DateReception` datetime(6) NULL,
        `Statut` int NOT NULL,
        `CreeParUtilisateurId` int NOT NULL,
        `NumeroCommande` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_CommandesFournisseur` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_CommandesFournisseur_Fournisseurs_FournisseurId` FOREIGN KEY (`FournisseurId`) REFERENCES `Fournisseurs` (`Id`) ON DELETE RESTRICT
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `RoleClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RoleId` int NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_RoleClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_RoleClaims_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `UtilisateurClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` int NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_UtilisateurClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_UtilisateurClaims_Utilisateurs_UserId` FOREIGN KEY (`UserId`) REFERENCES `Utilisateurs` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `UtilisateurLogins` (
        `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NOT NULL,
        CONSTRAINT `PK_UtilisateurLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
        CONSTRAINT `FK_UtilisateurLogins_Utilisateurs_UserId` FOREIGN KEY (`UserId`) REFERENCES `Utilisateurs` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `UtilisateurRoles` (
        `UserId` int NOT NULL,
        `RoleId` int NOT NULL,
        CONSTRAINT `PK_UtilisateurRoles` PRIMARY KEY (`UserId`, `RoleId`),
        CONSTRAINT `FK_UtilisateurRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_UtilisateurRoles_Utilisateurs_UserId` FOREIGN KEY (`UserId`) REFERENCES `Utilisateurs` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `UtilisateurTokens` (
        `UserId` int NOT NULL,
        `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Value` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_UtilisateurTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
        CONSTRAINT `FK_UtilisateurTokens_Utilisateurs_UserId` FOREIGN KEY (`UserId`) REFERENCES `Utilisateurs` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `LignesVente` (
        `Id` bigint NOT NULL AUTO_INCREMENT,
        `VenteId` bigint NOT NULL,
        `ProduitId` int NOT NULL,
        `Quantite` int NOT NULL,
        `PrixUnitaire` decimal(12,2) NOT NULL,
        `SousTotal` decimal(12,2) NOT NULL,
        CONSTRAINT `PK_LignesVente` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_LignesVente_Produits_ProduitId` FOREIGN KEY (`ProduitId`) REFERENCES `Produits` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_LignesVente_Ventes_VenteId` FOREIGN KEY (`VenteId`) REFERENCES `Ventes` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `MouvementsStock` (
        `Id` bigint NOT NULL AUTO_INCREMENT,
        `ProduitId` int NOT NULL,
        `Type` int NOT NULL,
        `Source` int NOT NULL,
        `Quantite` int NOT NULL,
        `Date` datetime(6) NOT NULL,
        `UtilisateurId` int NULL,
        `Reference` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Commentaire` varchar(250) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_MouvementsStock` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_MouvementsStock_Produits_ProduitId` FOREIGN KEY (`ProduitId`) REFERENCES `Produits` (`Id`) ON DELETE RESTRICT
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `ProduitFournisseurs` (
        `ProduitId` int NOT NULL,
        `FournisseurId` int NOT NULL,
        `PrixAchatFournisseur` decimal(12,2) NOT NULL,
        CONSTRAINT `PK_ProduitFournisseurs` PRIMARY KEY (`ProduitId`, `FournisseurId`),
        CONSTRAINT `FK_ProduitFournisseurs_Fournisseurs_FournisseurId` FOREIGN KEY (`FournisseurId`) REFERENCES `Fournisseurs` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ProduitFournisseurs_Produits_ProduitId` FOREIGN KEY (`ProduitId`) REFERENCES `Produits` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE TABLE `LignesCommandeFournisseur` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `CommandeId` int NOT NULL,
        `ProduitId` int NOT NULL,
        `QuantiteCommandee` int NOT NULL,
        `QuantiteRecue` int NOT NULL,
        `PrixUnitaire` decimal(12,2) NOT NULL,
        CONSTRAINT `PK_LignesCommandeFournisseur` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_LignesCommandeFournisseur_CommandesFournisseur_CommandeId` FOREIGN KEY (`CommandeId`) REFERENCES `CommandesFournisseur` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_LignesCommandeFournisseur_Produits_ProduitId` FOREIGN KEY (`ProduitId`) REFERENCES `Produits` (`Id`) ON DELETE RESTRICT
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Categories_Nom` ON `Categories` (`Nom`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_CommandesFournisseur_DateCommande` ON `CommandesFournisseur` (`DateCommande`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_CommandesFournisseur_FournisseurId` ON `CommandesFournisseur` (`FournisseurId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_CommandesFournisseur_NumeroCommande` ON `CommandesFournisseur` (`NumeroCommande`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_LignesCommandeFournisseur_CommandeId` ON `LignesCommandeFournisseur` (`CommandeId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_LignesCommandeFournisseur_ProduitId` ON `LignesCommandeFournisseur` (`ProduitId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_LignesVente_ProduitId` ON `LignesVente` (`ProduitId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_LignesVente_VenteId` ON `LignesVente` (`VenteId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_MouvementsStock_Date` ON `MouvementsStock` (`Date`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_MouvementsStock_ProduitId_Date` ON `MouvementsStock` (`ProduitId`, `Date`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_ProduitFournisseurs_FournisseurId` ON `ProduitFournisseurs` (`FournisseurId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_Produits_CategorieId` ON `Produits` (`CategorieId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Produits_CodeBarre` ON `Produits` (`CodeBarre`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_RoleClaims_RoleId` ON `RoleClaims` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE UNIQUE INDEX `RoleNameIndex` ON `Roles` (`NormalizedName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_UtilisateurClaims_UserId` ON `UtilisateurClaims` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_UtilisateurLogins_UserId` ON `UtilisateurLogins` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_UtilisateurRoles_RoleId` ON `UtilisateurRoles` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `EmailIndex` ON `Utilisateurs` (`NormalizedEmail`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE UNIQUE INDEX `UserNameIndex` ON `Utilisateurs` (`NormalizedUserName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_Ventes_CaissierId` ON `Ventes` (`CaissierId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE INDEX `IX_Ventes_DateHeure` ON `Ventes` (`DateHeure`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Ventes_NumeroTicket` ON `Ventes` (`NumeroTicket`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260802194415_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260802194415_InitialCreate', '8.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260803005205_AddNomClientToVente') THEN

    ALTER TABLE `Ventes` ADD `NomClient` varchar(150) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260803005205_AddNomClientToVente') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260803005205_AddNomClientToVente', '8.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

