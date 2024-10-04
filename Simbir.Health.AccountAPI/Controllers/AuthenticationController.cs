using Microsoft.AspNetCore.Mvc;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;

namespace Simbir.Health.AccountAPI.Controllers
{
    [Route("api/Authentication/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController() { }

        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] Auth_SignUp dtoObj)
        {

            //Добавить DatabaseSDK 
            List<string> roles_user = new List<string>() { "User" };

            UsersTable usersTable = new UsersTable()
            {
                firstName = dtoObj.firstName,
                lastName = dtoObj.lastName,
                password = dtoObj.password,
                username = dtoObj.username,
                roles = roles_user
            };

            using (DataContext db = new DataContext())
            {
                db.userTableObj.Add(usersTable);
                db.SaveChanges();
                return Ok();
            }

            //return BadRequest();
        }

    }
}
