using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Simbir.Health.HospitalAPI.Controllers;
using Simbir.Health.HospitalAPI.Model;
using Simbir.Health.HospitalAPI.SDK;
using Simbir.Health.HospitalAPI.SDK.Services;
using System.Security.Cryptography;

namespace Simbir.Health.Hospital
{
    public class Program
    {
        public static void Main(string[] args)
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
                Description = "����������� �� accessToken'�",
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
                    Title = "Hospitals API",
                    Description = "API �� ASP.NET, ��������� volga-it",
                    Contact = new OpenApiContact
                    {
                        Name = "����� (Study) - Developer",
                        Url = new Uri("https://github.com/StudyDevW")
                    }
                });

                //var basePath = AppContext.BaseDirectory;

                //var xmlPath = Path.Combine(basePath, "apidocs.xml");
                //o.IncludeXmlComments(xmlPath);
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

            builder.Services.AddHttpClient<HospitalsController>(o =>
            {
                o.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddSingleton<IDatabaseService, DatabaseSDK>();

            builder.Services.AddSingleton<ICacheService, CacheSDK>();

            //builder.Services.AddSingleton<IRabbitMQService, RabbitSDK>();

            //var db = new DataContext(builder.Configuration);

            //db.Database.Migrate();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospitals API");
                    c.RoutePrefix = "ui-swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}