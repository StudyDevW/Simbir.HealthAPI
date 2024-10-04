using Microsoft.EntityFrameworkCore;
using Simbir.Health.AccountAPI.Model;
using Simbir.Health.AccountAPI.Model.Database.DBO;

namespace Simbir.Health.AccountAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("ServerConn"));
            });

            var db = new DataContext();

            db.Database.Migrate();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}