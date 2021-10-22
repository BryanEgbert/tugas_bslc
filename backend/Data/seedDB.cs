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
			var roleExist = roleManager.RoleExistsAsync("Admin").Result;

			context.Database.EnsureCreated();
			if(!roleExist)
			{
				roleManager.CreateAsync(new IdentityRole("Admin"));
			}
			if (!context.Users.Any())
			{
				ApplicationUser user = new ApplicationUser()
				{
                    NIM = 2540120616,
					UserName = "JoeMama",
					Email = "test@gmail.com",
                    Password = "JoeMama123!"
				};

				userManager.CreateAsync(user, "JoeMama123!");
				userManager.AddToRoleAsync(user, "Admin");
			}
        }
    }
}