using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.SDK.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace Simbir.Health.AccountAPI.Controllers
{
    [Route("api/Authentication/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly IJwtService _jwt;

        private readonly ICacheService _cache;

        public AuthenticationController(IDatabaseService database, IJwtService jwt, ICacheService cache, IConfiguration configuration)
        {
            _database = database;
            _jwt = jwt;
            _cache = cache;
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
            if (dtoObj.username == null)
                return BadRequest();

            if (_database.CheckUser(dtoObj))
            {

                var accessToken = _jwt.JwtTokenCreation(dtoObj.username);
                var refreshToken = _jwt.RefreshTokenCreation(dtoObj.username);

                if (_cache.CheckExistKeysStorage(dtoObj.username, "accessTokens"))
                    _cache.DeleteKeyFromStorage(dtoObj.username, "accessTokens");

                if (_cache.CheckExistKeysStorage(dtoObj.username, "refreshTokens"))
                    _cache.DeleteKeyFromStorage(dtoObj.username, "refreshTokens");
                

                _cache.WriteKeyInStorage(dtoObj.username, "accessTokens", accessToken, DateTime.UtcNow.AddMinutes(2));
                _cache.WriteKeyInStorage(dtoObj.username, "refreshTokens", refreshToken, DateTime.UtcNow.AddMinutes(7));


                Auth_PairTokens pair_tokens = new Auth_PairTokens()
                {
                    accessToken = _cache.GetKeyFromStorage(dtoObj.username, "accessTokens"),
                    refreshToken = _cache.GetKeyFromStorage(dtoObj.username, "refreshTokens")
                };

                return Ok(pair_tokens);
            }

            return BadRequest();
        }

        [HttpGet("Validate")]
        public async Task<IActionResult> ValidateToken([FromHeader(Name = "accessToken")] string? token)
        {
            var validation = await _jwt.AccessTokenValidation("Bearer " + token);
           
            if (validation.TokenHasError())
            {
                return Unauthorized();
            }
            else if (validation.TokenHasSuccess())
            {
                return Ok($"Token for {validation.token_success.userName} is valid");
            }

            return BadRequest();
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPut("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key);

            if (validation.TokenHasError())
            {
                return Unauthorized();
            }
            else if (validation.TokenHasSuccess())
            {
                _cache.DeleteKeyFromStorage(validation.token_success.userName, "accessTokens");

                _cache.DeleteKeyFromStorage(validation.token_success.userName, "refreshTokens");

                return Ok($"{validation.token_success.userName} is logout");
            }

            return Unauthorized();
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshTokensPair([FromBody] Auth_Refresh dtoObj)
        {
            var validation = await _jwt.RefreshTokenValidation(dtoObj.refreshToken);

            if (validation.TokenHasError())
            {
                return Unauthorized();
            }
            else if (validation.TokenHasSuccess())
            {
                var accessToken = _jwt.JwtTokenCreation(validation.token_success.userName);
                var refreshToken = _jwt.RefreshTokenCreation(validation.token_success.userName);

                if (_cache.CheckExistKeysStorage(validation.token_success.userName, "accessTokens"))
                    _cache.DeleteKeyFromStorage(validation.token_success.userName, "accessTokens");

                if (_cache.CheckExistKeysStorage(validation.token_success.userName, "refreshTokens"))
                    _cache.DeleteKeyFromStorage(validation.token_success.userName, "refreshTokens");


                _cache.WriteKeyInStorage(validation.token_success.userName, "accessTokens", accessToken, DateTime.UtcNow.AddMinutes(2));
                _cache.WriteKeyInStorage(validation.token_success.userName, "refreshTokens", refreshToken, DateTime.UtcNow.AddDays(7));


                Auth_PairTokens pair_tokens = new Auth_PairTokens()
                {
                    accessToken = _cache.GetKeyFromStorage(validation.token_success.userName, "accessTokens"),
                    refreshToken = _cache.GetKeyFromStorage(validation.token_success.userName, "refreshTokens")
                };

                return Ok(pair_tokens);
            }

            return Unauthorized();
        }
    }
}
