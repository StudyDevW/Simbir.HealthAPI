using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Simbir.Health.HospitalAPI.Model.Database.DBO;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Simbir.Health.HospitalAPI.Model
{
    public class DataContext : DbContext
    {
        private readonly string _connectionString;

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options) {  }

        public DbSet<HospitalTable> hospitalTableObj { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }
    }
}
