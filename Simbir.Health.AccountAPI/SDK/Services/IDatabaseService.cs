using Simbir.Health.AccountAPI.Model.Database.DTO;

namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IDatabaseService
    {
        bool CheckUser(Auth_SignIn dto);
        public bool RegisterUser(Auth_SignUp dto);
    }
}
