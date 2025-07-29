using ApiBiblio.Database;
using ApiBiblio.Services.Integrations;
using ApiBiblio.Services.Interfaces;
using BibliothequeApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ApiBiblio
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddDbContextPool<BiblioDb>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo
                            {
                                Title = "BiblioSimplon",
                                Version = "v1",
                                Description = "Une Api pour gérer une bibliothèque",
                                Contact = new OpenApiContact
                                {
                                    Name = "BiblioSimplon",
                                    Email = "test.email@gmail.com"
                                }
                            });

                            

                            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"
                                        },
                                        Scheme = "oauth2",
                                        Name = "Bearer",
                                        In = ParameterLocation.Header,

                                    },
                                    new List<string>()
                                }
                            });

                        });

            // Configuration de l'authentification JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            builder.Services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = jwtSettings["Issuer"],
                                ValidAudience = jwtSettings["Audience"],
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                            };
                        });

            // Injection de dépendances pour les services
            builder.Services.AddScoped<ILivreService, LivreService>();
            builder.Services.AddScoped<IMembreService, MembreService>();
            builder.Services.AddScoped<IEmpruntService, EmpruntService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmployeService, EmployeService>();
            builder.Services.AddScoped<IAuteurService, AuteurService>();
            builder.Services.AddScoped<ICategorieService, CategorieService>();
            builder.Services.AddScoped<IGenreService, GenreService>();

            // Ajoutez les logs
            builder.Services.AddLogging();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Initialiser la base de données
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BiblioDb>();
                    await DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données.");
                }
            }

            app.UseSwagger();
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BiblioSimplon V1");
                            c.RoutePrefix = "swagger";
                        });

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Route MVC
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
