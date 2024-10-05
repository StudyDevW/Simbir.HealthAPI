using System.IdentityModel.Tokens.Jwt;

namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IJwtService
    {
        public string? JwtTokenCreation(IConfiguration conf, string userName);

        public string? RefreshTokenCreation(IConfiguration conf, string userName);

        public Task<JwtSecurityToken> RSAJwtValidation(IConfiguration conf, string? token);
    
    
    }
}
