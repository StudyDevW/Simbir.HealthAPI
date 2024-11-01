using Microsoft.EntityFrameworkCore;
using Simbir.Health.AccountAPI.Model.Database.DBO;

namespace Simbir.Health.AccountAPI.Model
{
    public class DataContext : DbContext
    {
        private readonly string _connectionString;

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options) {
        
        }

        public DbSet<UsersTable> userTableObj { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UsersTable>().HasData(
                new UsersTable() { id = 1, firstName = "Администратор", username = "admin", password = "admin", roles = new [] { "Admin" } },
                new UsersTable() { id = 2, firstName = "Менеджер", username = "manager", password = "manager", roles = new [] { "Manager" } },
                new UsersTable() { id = 3, firstName = "Доктор", username = "doctor", password = "doctor", roles = new [] { "Doctor" } },
                new UsersTable() { id = 4, firstName = "Пользователь", username = "user", password = "user", roles = new [] { "User" } }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
