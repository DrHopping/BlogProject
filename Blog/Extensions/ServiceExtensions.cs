﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Extensions
{
    public static class ServiceExtensions
    {
        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtFactory, JwtFactory>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ITagService, TagService>();
        }

        public static void AddBlogDb(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BlogDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        public static void UseIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<int>>(o =>
                {
                    o.User.RequireUniqueEmail = true;
                    o.Password.RequiredLength = 6;
                    o.Password.RequireLowercase = true;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireDigit = true;
                    o.Password.RequireNonAlphanumeric = false;
                }).AddEntityFrameworkStores<BlogDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {

            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            #region Old

            /*
            var appSettingsSection = configuration.GetSection("AppSettings");
            var secret = appSettingsSection.Get<JwtOptions>().Secret;
            var key = Encoding.ASCII.GetBytes(secret);
            var signingKey = new SymmetricSecurityKey(key);

            services.Configure<JwtOptions>(options =>
            {
                options.Secret = appSettingsSection["Secret"];
                options.Audience = appSettingsSection["Audience"];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //ValidIssuer = appSettingsSection["Issuer"],
                    //ValidAudience = appSettingsSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                };
                options.SaveToken = true;
                /*                options.Events = new JwtBearerEvents
                                {
                                    OnMessageReceived = context =>
                                    {
                                        if (context.Request.Query.ContainsKey("access_token"))
                                        {
                                            context.Token = context.Request.Query["access_token"];
                                        }

                                        return Task.CompletedTask;
                                    }
                                };#1#
            });
*/

            #endregion
        }

    }
}