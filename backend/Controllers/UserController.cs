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
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
		private readonly UserDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<ActionResult<LoginModel>> Login([FromBody]LoginModel model)
        {
            var userInfo = await _userManager.FindByEmailAsync(model.Email);
            if(userInfo != null)
            {
                var result = await _signInManager.PasswordSignInAsync(userInfo, model.Password, false, false);
                if(result.Succeeded)
                {
                    var claims = new[] 
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, userInfo.NIM.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, model.Email),
                        new Claim("role", model.Role)

                    };

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


                    return Ok(new {access_token = new JwtSecurityTokenHandler().WriteToken(token), exp = token.ValidTo});
                }
            }
            return NotFound("Wrong name or password");
        }

        [HttpPost]
        public async Task<ActionResult<LoginModel>> Register([FromBody]LoginModel model)
        {
            var userExist = await _userManager.FindByEmailAsync(model.Email);

            if (userExist != null)
            {
                return  StatusCode(500);  // return internal server error
            }
            ApplicationUser userInfo = new ApplicationUser 
            {
                NIM = model.NIM,
                Email = model.Email,
                Password = model.Password
            };

            var result = await _userManager.CreateAsync(userInfo, model.Password);
            if(result.Succeeded)
            {
				var signInResult = await _signInManager.PasswordSignInAsync(userInfo, model.Password, false, false);
				if (signInResult.Succeeded)
				{
					return CreatedAtAction(nameof(ApplicationUser), model);
				}
            }
            return Redirect("localhost:5500/frontend/index.html");
        }
    }
}