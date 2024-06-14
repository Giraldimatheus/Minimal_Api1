using Microsoft.EntityFrameworkCore;
using Minimal_Api1.Data;
using NetDevPack.Identity.Jwt;
using Microsoft.OpenApi.Models;

namespace Minimal_Api1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Minimal API",
                    Description = "Developed by Matheus Giraldi",
                    Contact = new OpenApiContact { Name = "Matheus Giraldi", Email = "mhgiraldi@outlook.com" },
                    License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                });
            });

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddDbContext<ChampionsContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentityEntityFrameworkContextConfiguration(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Minimal_Api1")));

            builder.Services.AddIdentityConfiguration();
            builder.Services.AddJwtConfiguration(builder.Configuration, "AppSettings");
            // Claims
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ExcluirFornecedor",
                    policy => policy.RequireClaim("ExcluirFornecedor"));
            });

            var app = builder.Build();

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthConfiguration();
            app.UseHttpsRedirection();

            Minimal_Api1.Endpoints.Endpoints.MapActions(app);

            app.Run();


        }
    }
}
