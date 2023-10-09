using ConclaseAcademyBlog.Configurations;
using ConclaseAcademyBlog.Data;
using ConclaseAcademyBlog.IRepository;
using ConclaseAcademyBlog.ProfileMapping;
using ConclaseAcademyBlog.Repository;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;

namespace ConclaseAcademyBlog.ApplicationExtensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();

            services.AddScoped<IUserContext, UserContext>();

            services.AddFluentValidationAutoValidation();

            services.Configure<JwtConfiguration>(_config.GetSection("JwtConfiguration"));

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(jwt =>
                {
                    var key = Encoding.ASCII.GetBytes(_config["JwtConfiguration:Secret"]);

                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false, //ToDO Update later
                        ValidateAudience = false, //ToDO Update later
                        RequireExpirationTime = false, //ToDO Update later
                        ValidateLifetime = false,
                    };
                });

            services.Configure<DataProtectionTokenProviderOptions>(option => option.TokenLifespan = TimeSpan.FromMinutes(30));

            services.AddIdentity<IdentityUser, IdentityRole>(option =>
            {
                option.SignIn.RequireConfirmedAccount = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }


}
