using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Alfa_1.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Alfa_1.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class TokenController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        private readonly AppConfiguration _appConfiguration;


        public TokenController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IOptions<AppConfiguration> appConfiguration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
            _appConfiguration = appConfiguration.Value;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AccountRegisterLogin model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(modelError => modelError.ErrorMessage).ToList());
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(x => x.Description).ToList());
            }

            await _signInManager.SignInAsync(user, false);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountRegisterLogin model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }


        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] AccountRegisterLogin model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) != PasswordVerificationResult.Success)
            {
                return BadRequest();
            }

            var token = await GetJwtSecurityToken(user);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }



        private async Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);


            var key = Encoding.UTF8.GetBytes(_appConfiguration.Key);
            var signingKey = new SymmetricSecurityKey(key);
            //   var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            return new JwtSecurityToken(
                issuer: _appConfiguration.SiteUrl,
                audience: _appConfiguration.SiteUrl,
                claims: GetTokenClaims(user).Union(userClaims),
                expires: DateTime.UtcNow.AddMinutes(10),
               signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Key)), SecurityAlgorithms.HmacSha256)
            );
        }

        private static IEnumerable<Claim> GetTokenClaims(ApplicationUser user)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
            };
        }


    }

    public class AccountRegisterLogin
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}