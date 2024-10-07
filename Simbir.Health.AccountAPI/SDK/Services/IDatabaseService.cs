using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.Model.Database.DTO.CheckUsers;

namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IDatabaseService
    {
        public Auth_CheckInfo CheckUser(Auth_SignIn dto);
        public bool RegisterUser(Auth_SignUp dto);

        public Accounts_Info InfoAccounts(int id);
    }
}
