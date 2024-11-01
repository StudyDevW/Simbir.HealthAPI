using System.ComponentModel.DataAnnotations;

namespace Simbir.Health.AccountAPI.Model.Database.DBO
{
    public class UsersTable
    {
        [Key]
        public int id { get; set; }
        public string? firstName {  get; set; }

        public string? lastName { get; set; }

        public string? username { get; set; }

        public string? password { get; set; }

        public string[]? roles { get; set; }
    }
}
