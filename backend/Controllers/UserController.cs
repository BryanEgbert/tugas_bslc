using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
                    return Ok(result);
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