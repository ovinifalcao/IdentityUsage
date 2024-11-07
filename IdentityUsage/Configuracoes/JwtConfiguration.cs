using Microsoft.IdentityModel.Tokens;

namespace IdentityUsage.Configuracoes
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public SigningCredentials SecurityKey { get; set; }
        public int AccessTokenExpiration { get; set; }
    }
}
