using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
	[ApiController]
	[Route("/Users/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult> GetUser()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("GetRoles")]
        public async Task<ActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpGet]
        [Route("GetUserRole")]
        public async Task<ActionResult> GetUserInRole(string roleName)
        {
            var user = await _userManager.GetUsersInRoleAsync(roleName);
            return Ok(user);
        }
    }
}