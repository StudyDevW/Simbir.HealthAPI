using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _conf;

        public DatabaseSDK(IConfiguration configuration) {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
            _conf = configuration;

        }

        public async Task RegisterUser(Auth_SignUp dto)
        {
            if (dto == null)
            {
                _logger.LogError("RegisterUser: dto==null");
                return;
            }

            string[] roles_user = new [] { "User" };

            UsersTable usersTable = new UsersTable()
            {
                firstName = dto.firstName,
                lastName = dto.lastName,
                password = dto.password,
                username = dto.username,
                roles = roles_user
            };

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                db.userTableObj.Add(usersTable);
                await db.SaveChangesAsync();

                _logger.LogInformation($"RegisterUser: {dto.firstName}, created");
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
                roles = dto.roles.ToArray()
            };

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                db.userTableObj.Add(usersTable);
                await db.SaveChangesAsync();

                _logger.LogInformation($"RegisterUserWithAdmin: {dto.firstName}, created");
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

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
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
                                roles = obj.roles.ToList()
                            } 
                        };
                }
            }

            _logger.LogError("CheckUser: username or password incorrect!");
            return new Auth_CheckInfo() { check_error = new Auth_CheckError { errorLog = "username/password_incorrect" } };
        }
        
        public Accounts_Info? InfoAccounts(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var selectedAcc = db.userTableObj.Where(c => c.id == id).FirstOrDefault();

                if (selectedAcc != null)
                {
                    return new Accounts_Info()
                    {
                        id = selectedAcc.id,
                        firstName = selectedAcc.firstName,
                        lastName = selectedAcc.lastName,
                        roles = selectedAcc.roles.ToList()
                    };
                }
            }

            return null;
        }

        public Accounts_Info? InfoAccountDoctor(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
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
                        roles = filtered_query.roles.ToList()
                    };
                }


            }

            return null;
        }

        public async Task UpdateAccount(Accounts_Update dto, int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var userToChange = db.userTableObj.Where(c => c.id == id).FirstOrDefault();
                    
                if (userToChange != null)
                {
                    if (userToChange.lastName != dto.lastName)
                    {
                        userToChange.lastName = dto.lastName;
                        await db.SaveChangesAsync();

                        _logger.LogInformation($"UpdateAccount: (id: {id} ) lastname was changed");
                    }
                    else
                    {
                        _logger.LogInformation($"UpdateAccount: (id: {id} ) lastname not changed");
                    }

                    if (userToChange.firstName != dto.firstName)
                    {
                        userToChange.firstName = dto.firstName; 
                        await db.SaveChangesAsync();

                        _logger.LogInformation($"UpdateAccount: (id: {id} ) firstName was changed");
                    }
                    else
                    {
                        _logger.LogInformation($"UpdateAccount: (id: {id} ) firstname not changed");
                    }

                    //Пароль не логирую
                    if (userToChange.password != dto.password)
                    {
                        userToChange.password = dto.password;
                        await db.SaveChangesAsync();
                    }
                }     
            }
        }

        public async Task UpdateAccountWithAdmin(Accounts_UpdateUser dto, int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var userToChange = db.userTableObj.Where(c => c.id == id).FirstOrDefault();

                if (userToChange != null)
                {
                    if (userToChange.lastName != dto.lastName)
                    {
                        userToChange.lastName = dto.lastName;
                        await db.SaveChangesAsync();

                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) lastname was changed");
                    }
                    else
                    {// (id: {id} ) 
                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) lastname not changed");
                    }

                    if (userToChange.firstName != dto.firstName)
                    {
                        userToChange.firstName = dto.firstName;
                        await db.SaveChangesAsync();


                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) firstname was changed");
                    }
                    else
                    {
                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) firstname not changed");
                    }

                    if (userToChange.username != dto.username)
                    {
                        userToChange.username = dto.username;
                        await db.SaveChangesAsync();

                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) username was changed");
                    }
                    else
                    {
                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) username not changed");
                    }

                    if (userToChange.password != dto.password)
                    {
                        userToChange.password = dto.password;
                        await db.SaveChangesAsync();
                    }

                    if (userToChange.roles.ToList() != dto.roles)
                    {
                        userToChange.roles = dto.roles.ToArray();
                        await db.SaveChangesAsync();

                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) roles was changed");
                    }
                    else
                    {
                        _logger.LogInformation($"UpdateAccountWithAdmin: (id: {id} ) roles not changed");
                    }
                }
            }
        }

        public async Task DeleteAccountWithAdmin(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var userToChange = db.userTableObj.Where(c => c.id == id).FirstOrDefault();

                if (userToChange != null)
                {
                    db.userTableObj.Remove(userToChange);
                    await db.SaveChangesAsync();

                    _logger.LogInformation($"DeleteAccountWithAdmin: (id: {id} ) was deleted");
                }
            }
        }


        public Accounts_GetAll GetAllAccounts(int _from, int _count)
        {
            Accounts_GetAll allAccounts = new Accounts_GetAll();

            allAccounts.Settings = new Accounts_SelectionSettings { from = _from, count = _count };
            //allAccounts.Content.Add(new Accounts_Info() { id = 0 });

            List<Accounts_Info> accounts = new List<Accounts_Info>();

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
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
                            roles = account.roles.ToList()
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
                            roles = account.roles.ToList()
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

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
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
                            roles = account.roles.ToList()
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
                            roles = account.roles.ToList()
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
