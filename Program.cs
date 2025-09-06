
//Program.cs
using FacesHunter.Data;
using FacesHunter.Helpers;
using FacesHunter.Models;
using FacesHunter.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace FacesHunter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- الخطوة 2: اجعل السيرفر يستقبل الطلبات من كل الـ IPs على البورت 5000
            builder.WebHost.UseUrls("http://0.0.0.0:5000");


            #region ✅ 1. Controllers + Enums as strings
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            #endregion

            #region ✅ 2. Swagger with JWT Support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "FacesHunter API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            #endregion

            #region ✅ 3. Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            #endregion

            #region ✅ 4. Services
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<AnalyticsService>();
            builder.Services.AddScoped<FaceSearchService>();
            builder.Services.AddScoped<FileService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            builder.Services.AddHttpClient<AiService>();
            builder.Services.AddHttpClient();
            #endregion

            #region ✅ 5. JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new Exception("JWT key is missing or empty in appsettings.json under Jwt:Key");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            #endregion

            #region ✅ 6. CORS for HTML/JS frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:3000",     // React
                        "http://127.0.0.1:5500"      // Live Server
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            #endregion

            #region ✅ 7. Upload size limits
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 104857600; // 100MB
            });
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 104857600;
            });
            #endregion

            var app = builder.Build();

            #region ✅ ✅ ✅ 8. Enable Swagger in All Environments
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FacesHunter API v1");
            });
            #endregion

            #region ✅ 9. Middleware
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowFrontend");
            #endregion

            #region ✅ 10. Static file mappings
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                RequestPath = "/images"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "success_videos")),
                RequestPath = "/success_videos"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "id_images")),
                RequestPath = "/id_images"
            });
            #endregion

            #region ✅ 11. Run Controllers
            app.MapControllers();
            #endregion

            #region ✅ 12. Seed persons from seed_faces folder if DB is empty
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var aiService = scope.ServiceProvider.GetRequiredService<AiService>();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                await SeedPersons.SeedAsync(db, aiService, env);
            }
            #endregion

            await app.RunAsync();
        }
    }
}
