namespace Simbir.Health.AccountAPI.Model.Database.DTO.ValidTokens
{
    public class Token_ValidSuccess
    {
        public int Id { get; set; }

        public string? userName { get; set; }

        public List<string>? userRoles { get; set; }

        public string? bearerWithoutPrefix { get; set; }
    }
}
