using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginModel>> Login([FromBody]LoginModel model)
        {
            var userInfo = await _userManager.FindByEmailAsync(model.Email);
            if(userInfo != null)
            {
				var result = await _signInManager.PasswordSignInAsync(userInfo, model.Password, false, false);
                if(result.Succeeded)
                {
                    var jwtToken = GenerateToken(userInfo);

                    return Ok(new {access_token = jwtToken});
                }
               
            }
            return NotFound("Wrong name or password");
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<LoginModel>> Register([FromBody]LoginModel model)
        {
            var userExist = await _userManager.FindByEmailAsync(model.Email);
            if(userExist != null)
            {
                return Forbid("User already exist");
            }
            ApplicationUser userInfo = new ApplicationUser 
            {
                NIM = model.NIM,
                UserName = model.Name,
                Email = model.Email,
                Password = model.Password
            };

            var result = await _userManager.CreateAsync(userInfo, model.Password);
            if(result.Succeeded)
            {
				await _userManager.AddToRoleAsync(userInfo, "Admin");
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
				claims.Add(new Claim(ClaimTypes.Role, userRole[i]));

				var role = await _roleManager.FindByNameAsync(userRole[i]);
				if (role != null)
				{
					var roleClaims = await _roleManager.GetClaimsAsync(role);   // Get role claims
					for (int j = 0; j < roleClaims.Count; ++i)
					{
						claims.Add(roleClaims[j]);  // Add role to claims
					}
				}
			}

            return claims;
        }

		private async Task<string> GenerateToken(ApplicationUser userInfo)
		{
			var claims = await GetValidClaims(userInfo);

			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("vwd84#gmb68nn+$(!80wu-n9u@b*!*bv(&$(b7-_yt_=l%9a!+"));
			var keyAlgorithm = SecurityAlgorithms.HmacSha256;

			var signingCredentials = new SigningCredentials(signingKey, keyAlgorithm);

			var token = new JwtSecurityToken(
				issuer: "http://localhost:5000",
				audience: "http://localhost:5500",
				claims: claims,
				notBefore: DateTime.Now,
				expires: DateTime.Now.AddDays(2),
				signingCredentials: signingCredentials
			);

			string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

			return jwtToken;
		}
	}
}