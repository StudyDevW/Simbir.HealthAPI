namespace Simbir.Health.AccountAPI.Model.Database.DTO.CheckUsers
{
    public class Auth_CheckSuccess
    {
        public int Id { get; set; }

        public string? username { get; set; }

        public List<string>? roles { get; set; }
    }
}
