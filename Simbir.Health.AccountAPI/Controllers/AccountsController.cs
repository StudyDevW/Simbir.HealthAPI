using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Health.AccountAPI.SDK.Services;

namespace Simbir.Health.AccountAPI.Controllers
{
    [Route("api/Accounts/")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        private readonly IDatabaseService _database;
        private readonly IJwtService _jwt;

        private readonly ICacheService _cache;

        public AccountsController(IDatabaseService database, IJwtService jwt, ICacheService cache, IConfiguration configuration)
        {
            _database = database;
            _jwt = jwt;
            _cache = cache;
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet("Me")]
        public async Task<IActionResult> GetProfileOfUser()
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key);

            if (validation.TokenHasError())
            {
                return Unauthorized();
            }
            else if (validation.TokenHasSuccess())
            {
                var info_user = _database.InfoAccounts(validation.token_success.Id);

                if (info_user != null)
                    return Ok(info_user);
            }

            return BadRequest();
        }

    }
}
