using System.ComponentModel.DataAnnotations;

namespace Simbir.Health.AccountAPI.Model.Database.DBO
{
    public class RolesTable
    {
        [Key]
        public int id_role { get; set; }

        public string? roles_key { get; set; }

        public string? name_role { get; set; }
    }
}
