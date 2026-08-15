namespace AlimentationDongmo.Api.Contracts;

public record LoginRequest(string Email, string MotDePasse);

public record LoginResponse(string Token, DateTime ExpireLe, UtilisateurDto Utilisateur);

public record UtilisateurDto(int Id, string NomComplet, string Email, IList<string> Roles);

public record MonProfilUpdateDto(string NomComplet, string Email);

public record ChangerMotDePasseDto(string AncienMotDePasse, string NouveauMotDePasse);

public record MotDePasseOublieDto(string Email);

public record ReinitialiserMotDePasseDto(string Email, string Token, string NouveauMotDePasse);
