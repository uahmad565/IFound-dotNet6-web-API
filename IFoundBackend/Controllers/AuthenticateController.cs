using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IFoundBackend.Auth;
using System.Linq;
using IFoundBackend.Model;
using IFoundBackend.Areas.Help;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using NuGet.Configuration;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net.Http;
using Microsoft.Extensions.Options;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IFoundBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("account/external-login")]
        public IActionResult ExternalLogin(string provider = "Google", string returnUrl = "")
        {
            var redirectUrl = $"https://localhost:44364/api/Authenticate/account/external-auth-callback?returnUrl={returnUrl}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.AllowRefresh = true;
            var result = Challenge(properties, provider);
            return result;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("account/external-auth-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "")
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            var result = await ExternalLogin(info);

            if (result == null)
            {
                return StatusCode(500, "An error occurred: External Login Failed");
            }

            Response.Cookies.Append("IFound_Token",
            JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            }));

            return Redirect(returnUrl);
        }

        public async Task<IActionResult> ExternalLogin(ExternalLoginInfo info)
        {
            var signinResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (signinResult.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var claims = JWTHandler.PrepareClaims(user, userRoles);

                var token = GetToken(claims);
                await _userManager.SetAuthenticationTokenAsync(
                    user,
                    TokenOptions.DefaultProvider,
                    "IFound_Token",
                    new JwtSecurityTokenHandler().WriteToken(token));

                return Ok(new
                {
                    x_auth_token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo.ToLocalTime().ToString("f"),
                    email = user.Email,
                    name = user.Name,
                });
            }

            if (!email.IsNullOrEmpty())
            {
                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        Name = info.Principal.FindFirstValue(ClaimTypes.Name)
                    };
                    await _userManager.CreateAsync(user);

                    if (await _roleManager.RoleExistsAsync(UserRoles.User))
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.User);
                    }
                }

                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, false);

                var token = GetToken(JWTHandler.PrepareClaims(user, new List<string> { UserRoles.User }));

                //sucess
                await _userManager.SetAuthenticationTokenAsync(
                    user,
                    TokenOptions.DefaultProvider,
                    "IFound_Token",
                    new JwtSecurityTokenHandler().WriteToken(token));

                return Ok(new
                {
                    x_auth_token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo.ToLocalTime().ToString("f"),
                    email = user.Email,
                    name = user.Name,
                });
            }

            return null;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier,user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims);
                    return Ok(new
                    {
                        x_auth_token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo.ToLocalTime().ToString("f"),
                        email = user.Email,
                        name = user.Name,
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new Auth.Response { Status = "Error", Message = ex.Message });

            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Email);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status400BadRequest, new Auth.Response { Status = "Error", Message = "User already exists!" });

                string userName = AuthenticateService.FindUserName(model.Email);
                ApplicationUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    UserName = userName
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Auth.Response { Status = "Error", Message = result.Errors.ElementAt(0).Description.ToString() });

                if (await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                var authClaims=JWTHandler.PrepareClaims(user, new List<string> { UserRoles.User });
                
                var token = GetToken(authClaims);
                return Ok(new
                {
                    x_auth_token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo.ToLocalTime().ToString("f"),
                    email = user.Email,
                    name = user.Name,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Auth.Response { Status = "Error", Message = ex.Message.ToString() });
            }

        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Auth.Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Auth.Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Auth.Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("verifyToken")]
        public IActionResult VerifyToken([FromBody] string token)
        {
            try
            {
                bool isValid = VerifyToken(token, _configuration["JWT:Secret"]);
                return new ObjectResult(new { isValid });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Auth.Response { Status = "Error", Message = "Token couldn't be verified." });
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }


        private bool VerifyToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = false, // Set to true if you want to validate the issuer
                ValidateAudience = false, // Set to true if you want to validate the audience
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Set the tolerance for expired tokens (optional)
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true; // Token is valid
            }
            catch (SecurityTokenException)
            {
                return false; // Token is invalid
            }
        }

    }
}
