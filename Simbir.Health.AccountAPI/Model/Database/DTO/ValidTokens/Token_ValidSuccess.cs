namespace Simbir.Health.AccountAPI.Model.Database.DTO.ValidTokens
{
    public class Token_ValidSuccess
    {
        public string? userName { get; set; }

        public List<string>? userRoles { get; set; }

        public string? bearerWithoutPrefix { get; set; }
    }
}
