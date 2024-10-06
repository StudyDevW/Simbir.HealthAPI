using Simbir.Health.AccountAPI.Model.Database.DTO.ValidTokens;
using System.IdentityModel.Tokens.Jwt;

namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IJwtService
    {
        public string JwtTokenCreation(string userName);

        public string RefreshTokenCreation(string userName);

       // public Task<JwtSecurityToken> RSAJwtValidation(string? token);

        public Task<Token_ValidProperties> AccessTokenValidation(string? bearerKey);

        public Task<Token_ValidProperties> RefreshTokenValidation(string? bearerKey);
    }
}
