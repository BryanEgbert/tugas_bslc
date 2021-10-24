using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Data
{
    public class SeedDB
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
			var context = serviceProvider.GetRequiredService<UserDbContext>();
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userRoleExist = roleManager.RoleExistsAsync("User").Result;
			var adminRoleExist = roleManager.RoleExistsAsync("Admin").Result;

			context.Database.EnsureCreated();
			if(!userRoleExist && !adminRoleExist)
			{
				roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();
				roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
			}
			if (!context.Users.Any())
			{
				ApplicationUser user = new ApplicationUser()
				{
                    NIM = 2540120616,
					UserName = "JoeMama",
					Email = "test@gmail.com"
				};

				userManager.CreateAsync(user, "JoeMama123!").GetAwaiter().GetResult();
				userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
			}
        }
    }
}