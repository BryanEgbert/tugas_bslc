using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using backend.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserDbContext _context;
		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, UserDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			Configuration = configuration;
			_context = context;
		}

		public IConfiguration Configuration { get; }

        [HttpPost]
        [Route("Login")]
		[Consumes("application/json")]
        public async Task<ActionResult<LoginModel>> Login([FromBody]LoginModel model)
        {
            var userInfo = await _userManager.FindByEmailAsync(model.Email);
            if(userInfo != null)
            {
                var result = await _signInManager.PasswordSignInAsync(userInfo, model.Password, false, false);
                if(result.Succeeded)
                {
                    var jwtToken = await GenerateToken(userInfo);

                    return Ok(jwtToken);
                }
            
            }
            return NotFound("Wrong name or password");
        }

        [HttpPost]
        [Route("Register")]
		[Consumes("application/json")]
        public async Task<ActionResult<LoginModel>> Register([FromBody]LoginModel model)
        {
            var userExist = await _userManager.FindByEmailAsync(model.Email);
            if(userExist != null)
            {
                return Forbid("User already exist");
            }
            ApplicationUser userInfo = new ApplicationUser 
            {
                NIM = UInt32.Parse(model.NIM),
                UserName = model.Name,
                Email = model.Email,
                PasswordHash = HashingService.PasswordHash(model.Password)
            };

            var result = await _userManager.CreateAsync(userInfo, model.Password);
            if(result.Succeeded)
            {
				await _userManager.AddToRoleAsync(userInfo, model.Role);
				return StatusCode(201, "User created");
            }
            return BadRequest("Failed to register user");
        }

        private async Task<List<Claim>> GetValidClaims(ApplicationUser userInfo)
        {
			var claims = new List<Claim>()
			{
				new Claim(JwtRegisteredClaimNames.Sub, userInfo.NIM.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			// Get user claims
			var userClaims = await _userManager.GetClaimsAsync(userInfo);
			claims.AddRange(userClaims);

			// Get user role
			var userRole = await _userManager.GetRolesAsync(userInfo);
			for (int i = 0; i < userRole.Count; ++i)
			{
				// Put role name to user claims
				claims.Add(new Claim("role", userRole[i]));

				var role = await _roleManager.FindByNameAsync(userRole[i]);
				if (role != null)
				{
					var roleClaims = await _roleManager.GetClaimsAsync(role);   // Get role claims
					for (int j = 0; j < roleClaims.Count; ++i)
					{
						claims.Add(new Claim("role", roleClaims[j].Value));  // Add role to claims
					}
				}
			}

            return claims;
        }

		private async Task<Object> GenerateToken(ApplicationUser userInfo)
		{
			var claims = await GetValidClaims(userInfo);

			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:key"]));
			var keyAlgorithm = SecurityAlgorithms.HmacSha256;

			var signingCredentials = new SigningCredentials(signingKey, keyAlgorithm);

			var token = new JwtSecurityToken(
				issuer: Configuration["Jwt:issuer"],
				audience: Configuration["Jwt:audience"],
				claims: claims,
				notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(5),
				signingCredentials: signingCredentials
			);

			return new { access_token = new JwtSecurityTokenHandler().WriteToken(token), exp = token.ValidTo };
		}
	}

}