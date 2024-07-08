using Assignment3.Data;
using Assignment3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;

        public LoginController(ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                User user = _applicationDbContext.Users.FirstOrDefault(s => s.Email == login.Email && s.Password == login.Password);

                if (user != null)
                {
                    var tokenString = GenerateJWT(user); // Generate JWT token
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Append("user-access-token", tokenString, cookieOptions);
                    return Ok(new { Token = tokenString });
                }
                else
                {
                    return Unauthorized(); // Invalid credentials
                }
            }
            else
            {
                return BadRequest(ModelState); // Invalid model state
            }
        }

        private string GenerateJWT(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, GetUserRole(user.UserID)) // Add user role to claims
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetUserRole(int userId)
        {
            var userRole = _applicationDbContext.UserRoles.FirstOrDefault(u => u.UserId == userId);
            if (userRole != null)
            {
                var role = _applicationDbContext.Roles.FirstOrDefault(r => r.RoleID == userRole.RoleId);
                if (role != null)
                {
                    return role.RoleName;
                }
            }
            return "Player"; // Default role if none found
        }
    }
}
