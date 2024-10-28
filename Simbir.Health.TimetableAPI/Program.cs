using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Simbir.Health.TimetableAPI.Controllers;
using Simbir.Health.TimetableAPI.Model;
using Simbir.Health.TimetableAPI.Model.Database;
using Simbir.Health.TimetableAPI.SDK;
using Simbir.Health.TimetableAPI.SDK.Services;
using System.Globalization;
using System.Security.Cryptography;

namespace Simbir.Health.TimetableAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Authorize accessToken",
            };

            var securityReq = new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            };

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(o =>
            {
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);

                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Timetable API",
                    Description = "ASP.NET core, Volga-it Olympiad",
                    Contact = new OpenApiContact
                    {
                        Name = "Sychenko Anton - Developer",
                        Url = new Uri("https://github.com/StudyDevW")
                    }
                });

                o.SchemaFilter<ExampleSchemaFilter>();
                var basePath = AppContext.BaseDirectory;

                var xmlPath = Path.Combine(basePath, "apidocsTimetable.xml");
                o.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "Asymmetric";
                o.DefaultChallengeScheme = "Asymmetric";
                o.DefaultScheme = "Asymmetric";

            }).AddJwtBearer("Asymmetric", o =>
            {
                o.IncludeErrorDetails = true;
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;

                var rsa = RSA.Create();

                var RSAPublicKey = () =>
                {
                    return @"-----BEGIN PUBLIC KEY-----
MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgHk1+4gUWP1RFPaIX3S1AHFShLat
EIkzvY1TvKF2HBhtZoM6XHUweQDF3AW32Ic9W+uvvkhqM87d8RPf5yT70BTyK+8F
VISvDDmVJ4+W5jU0zrxPOs/EuCSEaiL4pkJ5Z57PUwJs47Kkr/2AVqVtakAxozIP
BVVGSvbFKDiaJqprAgMBAAE=
-----END PUBLIC KEY-----";
                };

                rsa.ImportFromPem(RSAPublicKey());

                o.IncludeErrorDetails = true;
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;

                TokenValidationParameters tk_valid = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                    ValidateLifetime = false,
                    RequireExpirationTime = false //#
                };

                o.TokenValidationParameters = tk_valid;
            });

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("ServerConn"));
            });

            builder.Services.AddHttpClient<TimetableController>(o =>
            {
                o.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddHttpClient<AppointmentController>(o =>
            {
                o.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddSingleton<IDatabaseService, DatabaseSDK>();

            builder.Services.AddSingleton<ICacheService, CacheSDK>();
            //
  
            var app = builder.Build();

            await EnsureDatabaseInitializedAsync(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();   

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timetable API");
                    c.RoutePrefix = "ui-swagger";
                });
            }

            //перенаправление с корневого пути к swagger 
            app.Use(async (context, next) =>
            {

                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/ui-swagger/");
                }
                else
                {
                    await next();
                }
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseAuthentication();

            app.MapControllers();

            await app.RunAsync();

        }


        private static async Task<bool> CheckIfTableExistsAsync(DbContext context, string tableName)
        {
            var connection = (NpgsqlConnection)context.Database.GetDbConnection();
            await connection.OpenAsync();

            var exists = false;

            var command = new NpgsqlCommand(
                $"SELECT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = '{tableName}');",
                connection);

            exists = (bool)await command.ExecuteScalarAsync();

            await connection.CloseAsync();
            return exists;
        }

        private static async Task EnsureDatabaseInitializedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            var tableNameF = "timeTableObj";
            var tableNameS = "appointmentsTableObj";

            var tableExistsFirst = await CheckIfTableExistsAsync(context, tableNameF);

            var tableExistsSecond = await CheckIfTableExistsAsync(context, tableNameS);

            if (!tableExistsFirst && !tableExistsSecond)
            {
                await context.Database.MigrateAsync();
            }
        }
    }
}