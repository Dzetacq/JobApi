using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobApi.Model;
using Microsoft.EntityFrameworkCore;

namespace JobApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            User? user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) 
                return Unauthorized();
            user = _userManager.Users.Include(u => u.Companies).Include(u => u.Applications)
                .First(u => u.Id == user.Id);
            IList<string>? userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> authClaims = new()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            JwtSecurityToken token = GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user
            });
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput model)
        {
            User? userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username, 
                Firstname = model.FirstName,
                Lastname = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                LinkedIn = model.LinkedIn
            };
            IdentityResult? result = await _userManager.CreateAsync(user, model.Password);
            return !result.Succeeded 
                ? StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error", Message = "User creation failed! Please check details and try again."
                }) 
                : Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPut]
        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] EditInput model)
        {
            User? user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exist!" });

            if (user.UserName != model.Username && model.Username is { Length: > 0 } && await _userManager.FindByNameAsync(model.Username) != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Username already taken!" });
            }
            if (user.Email != model.Email && model.Email is { Length: > 0}
                && await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email already taken!" });
            }

            user.UserName = model.Username is { Length: > 0 } ? model.Username : user.UserName;
            user.Email = model.Email is { Length: > 0 } ? model.Email : user.Email;
            user.Firstname = model.FirstName;
            user.Lastname = model.LastName is { Length: > 0 } ? model.LastName : user.Lastname;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.LinkedIn = model.LinkedIn;
            IdentityResult? result = await _userManager.UpdateAsync(user);
            return !result.Succeeded
                ? StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User modification failed! Please check details and try again."
                })
                : Ok(new Response { Status = "Success", Message = "User modified successfully!" });
        }

        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            SymmetricSecurityKey authSigningKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            JwtSecurityToken token = new(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpPut]
        [Route("password/{id}")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] PwModel model)
        {
            User? user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Unauthorized();

            string? token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult? result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "Your password isn't strong enough."
                });
            }
            return Ok(new Response { Status = "Success", Message = "User modified successfully!" });
        }
    }
}
