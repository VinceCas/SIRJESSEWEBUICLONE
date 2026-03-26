using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {

        private readonly string _secretKey = "THIS_IS_A_SAMPLE_KEY_THIS_IS_A_SAMPLE_KEY";

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var permissions = new List<string>();//container for the permissions granted for user

            if (request.Email == "admin@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_products", "add_products", "edit_products", "delete_products" });//adds the permission before generating the token for authentication

            }
            else if (request.Email == "view@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_products", "add_products"});//adds the permission before generating the token for authentication

            }
            else if (request.Email == "viewedit@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_products", "add_products","edit_products" });//adds the permission before generating the token for authentication

            }
            else
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(request.Email, permissions);

            return Ok(new
            {
                token = token,
                email = request.Email
            });
        }

        private string GenerateJwtToken(string email, List<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>();

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}