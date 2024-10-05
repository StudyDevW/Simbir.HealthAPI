using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.SDK.Services;

namespace Simbir.Health.AccountAPI.SDK
{
    public class DatabaseSDK : IDatabaseService
    {
        public DatabaseSDK() { 
        
        }



        public void RegisterUser(Auth_SignUp dto)
        {
            if (dto == null)
                return;

            List<string> roles_user = new List<string>() { "User" };

            UsersTable usersTable = new UsersTable()
            {
                firstName = dto.firstName,
                lastName = dto.lastName,
                password = dto.password,
                username = dto.username,
                roles = roles_user
            };

            using (DataContext db = new DataContext())
            {
                db.userTableObj.Add(usersTable);
                db.SaveChanges();
            }
        }
    }
}
