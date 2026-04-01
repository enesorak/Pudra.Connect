using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pudra.Connect.Application.Interfaces;
using Pudra.Connect.Infrastructure.Persistence;
using Pudra.Connect.Infrastructure.Repositories;
using Pudra.Connect.Infrastructure.Services;

namespace Pudra.Connect.Api.Extensions;

public static class DependencyInjection
    {
        // Bu metot, Infrastructure katmanına ait servisleri kaydeder.
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Veritabanı bağlantısı
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)); 

            // Kendi yazdığımız servisler
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        // Bu metot, API katmanına ait servisleri ve Authentication'ı kaydeder.
        public static IServiceCollection AddApiServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
          

            services.AddSwaggerGen(options =>
            {
                // 1. Güvenlik Şemasını Tanımla
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header'ını Bearer şemasıyla girin. \r\n\r\n Örnek: \"Bearer 12345abcdef\""
                });

                // 2. Güvenlik Gereksinimini Ekle
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            
            
            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
                };
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>(); // ProductService'i birazdan oluşturacağız.
 
 
            services.AddMemoryCache(); // <-- BU SATIRI EKLİYORUZ

            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IUserService, UserService>();

            
            services.AddScoped<IScanAnalyticsService, ScanAnalyticsService>();
            
            return services;
        }
    }