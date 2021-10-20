using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Data
{
    public class seedDB
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
			var context = serviceProvider.GetRequiredService<UserDbContext>();
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			context.Database.EnsureCreated();
			if (!context.Users.Any())
			{
				ApplicationUser user = new ApplicationUser()
				{
                    NIM = 2540120616,
					Email = "test@gmail.com",
                    Password = "JoeMama123",
                    Role = "user"
				};
                
				userManager.CreateAsync(user, "Test@123");
			}
        }
    }
}