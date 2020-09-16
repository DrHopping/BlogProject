using System.Linq;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Extensions
{
    public static class AppExtensions
    {
        public static async Task CreateSeedData(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {

                var RoleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole<int>>>();
                string[] roles = { "Admin", "Moderator", "RegularUser" };

                foreach (string role in roles)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(role);
                    if (!roleExist)
                    {
                        await RoleManager.CreateAsync(new IdentityRole<int>(role));
                    }
                }

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                var powerUser = new User
                {
                    UserName = configuration.GetSection("AdminSettings")["UserName"],
                    Email = configuration.GetSection("AdminSettings")["UserEmail"]
                };

                string userPassword = configuration.GetSection("AdminSettings")["UserPassword"];
                var user = await userManager.FindByEmailAsync(configuration.GetSection("AdminSettings")["UserEmail"]);
                if (user == null)
                {
                    var createPowerUser = await userManager.CreateAsync(powerUser, userPassword);
                    if (createPowerUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(powerUser, "Admin");
                    }
                }
            }
        }

        public static IApplicationBuilder EnsureDbCreated(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<BlogDbContext>();
            context.Database.Migrate();
            return app;
        }
    }
}