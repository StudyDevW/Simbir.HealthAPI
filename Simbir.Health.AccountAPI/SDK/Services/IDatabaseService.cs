using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.Model.Database.DTO.AccountSelect;
using Simbir.Health.AccountAPI.Model.Database.DTO.CheckUsers;

namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IDatabaseService
    {
        public Auth_CheckInfo CheckUser(Auth_SignIn dto);

        public Task RegisterUser(Auth_SignUp dto);

        public Accounts_Info? InfoAccounts(int id);

        public Task UpdateAccount(Accounts_Update dto, int id);

        public Accounts_GetAll GetAllAccounts(int from, int count);

        public Task RegisterUserWithAdmin(Accounts_CreateUser dto);

        public Task UpdateAccountWithAdmin(Accounts_UpdateUser dto, int id);

        public Task DeleteAccountWithAdmin(int id);

        public Accounts_GetAll GetAllDoctors(int _from, int _count, string nameFilter);

        public Accounts_Info? InfoAccountDoctor(int id);
    }
}
