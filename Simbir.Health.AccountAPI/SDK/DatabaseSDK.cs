using Microsoft.AspNetCore.Mvc;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.SDK.Services;

namespace Simbir.Health.AccountAPI.SDK
{
    public class DatabaseSDK : IDatabaseService
    {
        private readonly ILogger _logger;

        public DatabaseSDK() {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
        }



        public bool RegisterUser(Auth_SignUp dto)
        {
            if (dto == null)
            {
                _logger.LogError("RegisterUser: dto==null");
                return false;
            }

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

                _logger.LogInformation($"RegisterUser: {dto.firstName}, зарегистрировался!");
            }

            return true;
        }

        public bool CheckUser(Auth_SignIn dto)
        {
            if (dto == null)
            {
                _logger.LogError("CheckUser: dto==null");
                return false;
            }

            using (DataContext db = new DataContext())
            {
                foreach (var obj in db.userTableObj)
                {
                    if (obj.username == dto.username &&
                        obj.password == dto.password)
                        return true;
                }
            }

            _logger.LogError("CheckUser: Неправильное имя пользователя или пароль!");
            return false;
        }
    }
}
