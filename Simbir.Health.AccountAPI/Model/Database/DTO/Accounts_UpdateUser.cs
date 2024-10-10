namespace Simbir.Health.AccountAPI.Model.Database.DTO
{
    public class Accounts_UpdateUser
    {
        public string? lastName { get; set; }

        public string? firstName { get; set; }

        public string? username { get; set; }

        public string? password { get; set; }

        public List<string>? roles { get; set; }
    }
}
