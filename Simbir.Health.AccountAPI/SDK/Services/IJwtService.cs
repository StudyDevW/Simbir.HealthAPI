using Simbir.Health.AccountAPI.Model.Database.DTO.CheckUsers;
using Simbir.Health.AccountAPI.Model.Database.DTO.ValidTokens;
using System.IdentityModel.Tokens.Jwt;

namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IJwtService
    {
        public string JwtTokenCreation(Auth_CheckSuccess dtoObj);

        public string RefreshTokenCreation(Auth_CheckSuccess dtoObj);

       // public Task<JwtSecurityToken> RSAJwtValidation(string? token);

        public Task<Token_ValidProperties> AccessTokenValidation(string? bearerKey);

        public Task<Token_ValidProperties> RefreshTokenValidation(string? bearerKey);
    }
}
