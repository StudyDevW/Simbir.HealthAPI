using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.Model.Database.DTO.AccountSelect;
using Simbir.Health.AccountAPI.Model.Database.DTO.CheckUsers;
using Simbir.Health.AccountAPI.SDK.Services;
using System.Linq;

namespace Simbir.Health.AccountAPI.SDK
{
    public class DatabaseSDK : IDatabaseService
    {
        private readonly ILogger _logger;

        public DatabaseSDK() {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
        }

        public async Task RegisterUser(Auth_SignUp dto)
        {
            if (dto == null)
            {
                _logger.LogError("RegisterUser: dto==null");
                return;
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
                await db.SaveChangesAsync();

                _logger.LogInformation($"RegisterUser: {dto.firstName}, зарегистрировался!");
            }

            return;
        }

        public async Task RegisterUserWithAdmin(Accounts_CreateUser dto)
        {
            if (dto == null)
            {
                _logger.LogError("RegisterUser: dto==null");
                return;
            }

            UsersTable usersTable = new UsersTable()
            {
                firstName = dto.firstName,
                lastName = dto.lastName,
                password = dto.password,
                username = dto.username,
                roles = dto.roles
            };

            using (DataContext db = new DataContext())
            {
                db.userTableObj.Add(usersTable);
                await db.SaveChangesAsync();

                _logger.LogInformation($"RegisterUserWithAdmin: {dto.firstName}, создан!");
            }

            return;
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
                            id = obj.id,
                            firstName = obj.firstName,
                            lastName = obj.lastName,
                            roles = obj.roles
                        };
                }
            }

            return new Accounts_Info();
        }

        public Accounts_Info InfoAccountDoctor(int id)
        {
            using (DataContext db = new DataContext())
            {
                var filtered_query = db.userTableObj.Where(o => o.id == id && o.roles.Contains("Doctor"))
                    .FirstOrDefault();

                if (filtered_query != null)
                {
                    return new Accounts_Info()
                    {
                        id = filtered_query.id,
                        firstName = filtered_query.firstName,
                        lastName = filtered_query.lastName,
                        roles = filtered_query.roles
                    };
                }


            }

            return new Accounts_Info();
        }

        public async Task UpdateAccount(Accounts_Update dto, int id)
        {
            using (DataContext db = new DataContext())
            {
                var userToChange = db.userTableObj.Where(c => c.id == id).FirstOrDefault();
                    
                if (userToChange != null)
                {
                    if (userToChange.lastName != dto.lastName)
                    {
                        userToChange.lastName = dto.lastName;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAccount: lastname not changed");
                    }

                    if (userToChange.firstName != dto.firstName)
                    {
                        userToChange.firstName = dto.firstName; 
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAccount: firstname not changed");
                    }

                    if (userToChange.password != dto.password)
                    {
                        userToChange.password = dto.password;
                        await db.SaveChangesAsync();
                    }
                    //else
                    //{
                        //Не логируем пароль, так как это опасно!
                    //}
                }     
            }
        }

        public async Task UpdateAccountWithAdmin(Accounts_UpdateUser dto, int id)
        {
            using (DataContext db = new DataContext())
            {
                var userToChange = db.userTableObj.Where(c => c.id == id).FirstOrDefault();

                if (userToChange != null)
                {
                    if (userToChange.lastName != dto.lastName)
                    {
                        userToChange.lastName = dto.lastName;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAccountWithAdmin: lastname not changed");
                    }

                    if (userToChange.firstName != dto.firstName)
                    {
                        userToChange.firstName = dto.firstName;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAccountWithAdmin: firstname not changed");
                    }

                    if (userToChange.username != dto.username)
                    {
                        userToChange.username = dto.username;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAccountWithAdmin: username not changed");
                    }

                    if (userToChange.password != dto.password)
                    {
                        userToChange.password = dto.password;
                        await db.SaveChangesAsync();
                    }

                    if (userToChange.roles != dto.roles)
                    {
                        userToChange.roles = dto.roles;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAccountWithAdmin: roles not changed");
                    }
                }
            }
        }

        public async Task DeleteAccountWithAdmin(int id)
        {
            using (DataContext db = new DataContext())
            {
                var userToChange = db.userTableObj.Where(c => c.id == id).FirstOrDefault();

                if (userToChange != null)
                {
                    db.userTableObj.Remove(userToChange);
                    await db.SaveChangesAsync();
                }
            }
        }


        public Accounts_GetAll GetAllAccounts(int _from, int _count)
        {
            Accounts_GetAll allAccounts = new Accounts_GetAll();

            allAccounts.Settings = new Accounts_SelectionSettings { from = _from, count = _count };
            //allAccounts.Content.Add(new Accounts_Info() { id = 0 });

            List<Accounts_Info> accounts = new List<Accounts_Info>();

            using (DataContext db = new DataContext())
            {
                if (_count != 0)
                {
                    var filtered_query = db.userTableObj.Skip(_from).Take(_count);

                    foreach (var account in filtered_query)
                    {
                        Accounts_Info accountInfo = new Accounts_Info()
                        {
                            id = account.id,
                            firstName = account.firstName,
                            lastName = account.lastName,
                            roles = account.roles
                        };

                        accounts.Add(accountInfo);
                    }
                }
                else
                {
                    var filtered_query = db.userTableObj.Skip(_from);

                    foreach (var account in filtered_query)
                    {
                        Accounts_Info accountInfo = new Accounts_Info()
                        {
                            id = account.id,
                            firstName = account.firstName,
                            lastName = account.lastName,
                            roles = account.roles
                        };

                        accounts.Add(accountInfo);
                    }
                }
            }

            allAccounts.ContentFill(accounts);

            return allAccounts;
        }

        public Accounts_GetAll GetAllDoctors(int _from, int _count, string nameFilter)
        {
            Accounts_GetAll allAccounts = new Accounts_GetAll();

            allAccounts.Settings = new Accounts_SelectionSettings { from = _from, count = _count };

            List<Accounts_Info> accounts = new List<Accounts_Info>();

            using (DataContext db = new DataContext())
            {
                if (_count != 0)
                {
                    var filtered_query = db.userTableObj.Where(o => o.roles.Contains("Doctor") && (o.firstName.Contains(nameFilter) || o.lastName.Contains(nameFilter)))
                        .Skip(_from)
                        .Take(_count);

                    foreach (var account in filtered_query)
                    {
                        Accounts_Info accountInfo = new Accounts_Info()
                        {
                            id = account.id,
                            firstName = account.firstName,
                            lastName = account.lastName,
                            roles = account.roles
                        };

                        accounts.Add(accountInfo);
                    }
                }
                else
                {
                    var filtered_query = db.userTableObj.Where(o => o.roles.Contains("Doctor") && (o.firstName.Contains(nameFilter) || o.lastName.Contains(nameFilter)))
                       .Skip(_from);

                    foreach (var account in filtered_query)
                    {
                        Accounts_Info accountInfo = new Accounts_Info()
                        {
                            id = account.id,
                            firstName = account.firstName,
                            lastName = account.lastName,
                            roles = account.roles
                        };

                        accounts.Add(accountInfo);
                    }
                }
            }

            allAccounts.ContentFill(accounts);

            return allAccounts;
        }


    }
}
