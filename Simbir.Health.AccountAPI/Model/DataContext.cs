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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersTable>().HasData(new List<UsersTable>
            {
                new UsersTable(){ id = 1, firstName = "Администратор", username = "admin", password = "admin", roles = new List<string> { "Admin" } },
                new UsersTable(){ id = 2, firstName = "Менеджер", username = "manager", password = "manager", roles = new List<string> { "Manager" } },
                new UsersTable(){ id = 3, firstName = "Доктор", username = "doctor", password = "doctor", roles = new List<string> { "Doctor" } },
                new UsersTable(){ id = 4, firstName = "Пользователь", username = "user", password = "user", roles = new List<string> { "User" } }
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
