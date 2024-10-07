using Microsoft.AspNetCore.Mvc;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.Model.Database.DTO.CheckUsers;
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

        public Auth_CheckInfo CheckUser(Auth_SignIn dto)
        {
            if (dto == null)
            {
                _logger.LogError("CheckUser: dto==null");
                return new Auth_CheckInfo() { check_error = new Auth_CheckError { errorLog = "input_incorrect" } };
            }

            using (DataContext db = new DataContext())
            {
                foreach (var obj in db.userTableObj)
                {
                    if (obj.username == dto.username &&
                        obj.password == dto.password)
                        return new Auth_CheckInfo() {
                            check_success = new Auth_CheckSuccess
                            {
                                Id = obj.id,
                                username = obj.username,
                                roles = obj.roles
                            } 
                        };
                }
            }

            _logger.LogError("CheckUser: Неправильное имя пользователя или пароль!");
            return new Auth_CheckInfo() { check_error = new Auth_CheckError { errorLog = "username/password_incorrect" } };
        }
        
        public Accounts_Info InfoAccounts(int id)
        {

            using (DataContext db = new DataContext())
            {
                foreach (var obj in db.userTableObj)
                {
                    if (obj.id == id)
                        return new Accounts_Info()
                        {
                            firstName = obj.firstName,
                            lastName = obj.lastName,
                            roles = obj.roles
                        };
                }
            }

            return new Accounts_Info();
        }
    }
}
