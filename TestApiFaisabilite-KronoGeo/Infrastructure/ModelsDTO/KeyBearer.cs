namespace TestApiFaisabilite_KronoGeo.Infrastructure.ModelsDTO
{
    public class KeyBearer
    {
        public string Key { get; set; } = string.Empty;
        public bool ValidateAudience { get; set; } = false; // - Pour s'assurer que le token est destiné à notre API pou l'audiance du pays
                                                            // - url de l'audience du token (ex: https://api.monsite.com)
        public bool ValidateIssuer { get; set; } = false; // - Pour s'assurer que le token a été émis par une source de confiance (ex: notre serveur d'authentification)
                                                          // - url de l'autorité d'émission du token (ex: https://auth.monsite.com)
        public bool ValidateActor { get; set; } = false;  // - valider l'acteur qui est à l'origine de la demande d'authentification OAuth2.0
        public bool ValidateLifetime { get; set; } = true;
    }
}
