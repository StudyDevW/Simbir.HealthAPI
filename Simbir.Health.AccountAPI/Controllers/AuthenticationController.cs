using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.Model.Database.DTO.CheckUsers;
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

            var check = _database.CheckUser(dtoObj);

            if (check.CheckHasSuccess())
            {
                var accessToken = _jwt.JwtTokenCreation(check.check_success);
                var refreshToken = _jwt.RefreshTokenCreation(check.check_success);

                if (_cache.CheckExistKeysStorage(check.check_success.Id, "accessTokens"))
                    _cache.DeleteKeyFromStorage(check.check_success.Id, "accessTokens");

                if (_cache.CheckExistKeysStorage(check.check_success.Id, "refreshTokens"))
                    _cache.DeleteKeyFromStorage(check.check_success.Id, "refreshTokens");
                

                _cache.WriteKeyInStorage(check.check_success.Id, "accessTokens", accessToken, DateTime.UtcNow.AddMinutes(2));
                _cache.WriteKeyInStorage(check.check_success.Id, "refreshTokens", refreshToken, DateTime.UtcNow.AddMinutes(7));


                Auth_PairTokens pair_tokens = new Auth_PairTokens()
                {
                    accessToken = _cache.GetKeyFromStorage(check.check_success.Id, "accessTokens"),
                    refreshToken = _cache.GetKeyFromStorage(check.check_success.Id, "refreshTokens")
                };

                return Ok(pair_tokens);
            }

            return BadRequest();
        }

        [HttpGet("Validate")]
        public async Task<IActionResult> ValidateToken([FromHeader(Name = "accessToken")] string? token)
        {
            if (token != null)
            {
                if (token.Contains("Bearer"))
                    return BadRequest("accessToken in this method must not contain word [Bearer]");
            }

            var validation = await _jwt.AccessTokenValidation("Bearer " + token);
           
            if (validation.TokenHasError())
            {
                return Unauthorized();
            }
            else if (validation.TokenHasSuccess())
            {
                return Ok($"Token for id_user: {validation.token_success.Id} is valid");
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

                _cache.DeleteKeyFromStorage(validation.token_success.Id, "accessTokens");

                _cache.DeleteKeyFromStorage(validation.token_success.Id, "refreshTokens");

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
                Auth_CheckSuccess authsuccess = new Auth_CheckSuccess()
                {
                    Id = validation.token_success.Id,
                    roles = validation.token_success.userRoles,
                    username = validation.token_success.userName
                };

                var accessToken = _jwt.JwtTokenCreation(authsuccess);
                var refreshToken = _jwt.RefreshTokenCreation(authsuccess);

                if (_cache.CheckExistKeysStorage(authsuccess.Id, "accessTokens"))
                    _cache.DeleteKeyFromStorage(authsuccess.Id, "accessTokens");

                if (_cache.CheckExistKeysStorage(authsuccess.Id, "refreshTokens"))
                    _cache.DeleteKeyFromStorage(authsuccess.Id, "refreshTokens");


                _cache.WriteKeyInStorage(authsuccess.Id, "accessTokens", accessToken, DateTime.UtcNow.AddMinutes(2));
                _cache.WriteKeyInStorage(authsuccess.Id, "refreshTokens", refreshToken, DateTime.UtcNow.AddDays(7));


                Auth_PairTokens pair_tokens = new Auth_PairTokens()
                {
                    accessToken = _cache.GetKeyFromStorage(authsuccess.Id, "accessTokens"),
                    refreshToken = _cache.GetKeyFromStorage(authsuccess.Id, "refreshTokens")
                };

                return Ok(pair_tokens);
            }

            return Unauthorized();
        }

    }
}
