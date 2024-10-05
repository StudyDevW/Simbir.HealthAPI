using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.SDK.Services;

namespace Simbir.Health.AccountAPI.Controllers
{
    [Route("api/Authentication/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly IJwtService _jwt;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IDatabaseService database, IJwtService jwt, IConfiguration configuration)
        {
            _database = database;
            _jwt = jwt;
            _configuration = configuration;
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] Auth_SignUp dtoObj)
        {
            if (_database.RegisterUser(dtoObj))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("SignIn")]
        public IActionResult SignIn([FromBody] Auth_SignIn dtoObj)
        {
            if (_database.CheckUser(dtoObj))
            {

                var accessToken = _jwt.JwtTokenCreation(_configuration, dtoObj.username);

                var refreshToken = _jwt.RefreshTokenCreation(_configuration, dtoObj.username);

                Auth_PairTokens pair_tokens = new Auth_PairTokens()
                {
                    accessToken = accessToken,
                    refreshToken = refreshToken
                };

                return Ok(pair_tokens);
            }

            return BadRequest();
        }

        [HttpGet("Validate")]
        public async Task<IActionResult> ValidateTokenAsync([FromHeader(Name = "accessToken")] string? token)
        {
            var validation = await _jwt.RSAJwtValidation(_configuration, token);
            var expectedAlg = SecurityAlgorithms.RsaSha512;

            if (validation == null)
            {
                return BadRequest();
            }
            else {
                if (validation.Header?.Alg == null || validation.Header?.Alg != expectedAlg)
                {
                    return BadRequest("Unexpected Alg");
                }

                return Ok("Token Valid");
            }

     
        }

    }
}
