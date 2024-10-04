using Microsoft.EntityFrameworkCore;
using Simbir.Health.AccountAPI.Model.Database.DBO;

namespace Simbir.Health.AccountAPI.Model
{
    public class DataContext : DbContext
    {
        public DataContext() => Database.EnsureCreated();

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<UsersTable> userTableObj { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=postgres_db;Database=simbirhealth;Port=5432;User Id=volgait_practice;Password=root;");
            }
        }
    }
}
