using EmployeeManagement.Api.DTO;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using SqlConnection con = new SqlConnection(connectionString);

            using SqlCommand checkCmd =
                new SqlCommand("sp_GetUserByEmail", con);

            checkCmd.CommandType = CommandType.StoredProcedure;

            checkCmd.Parameters.AddWithValue("@Email", dto.Email);

            await con.OpenAsync();

            using SqlDataReader reader =
                await checkCmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return BadRequest("Email already exists.");
            }

            await reader.CloseAsync();

            var user = new User
            {
                Email = dto.Email,
                Role = dto.Role,
                IsActive = true
            };

            var passwordHasher = new PasswordHasher<User>();

            string passwordHash =
                passwordHasher.HashPassword(user, dto.Password);

            using SqlCommand cmd =
                new SqlCommand("sp_RegisterUser", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", dto.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@Role", dto.Role);

            await cmd.ExecuteNonQueryAsync();

            return Ok(new
            {
                message = "User registered successfully."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using SqlConnection con = new SqlConnection(connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_GetUserByEmail", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", dto.Email);

            await con.OpenAsync();

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Unauthorized("Invalid email or password.");
            }

            var user = new User
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                Email = reader["Email"].ToString()!,
                PasswordHash = reader["PasswordHash"].ToString()!,
                Role = reader["Role"].ToString()!,
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };

            if (!user.IsActive)
            {
                return Unauthorized("User is inactive.");
            }

            var passwordHasher = new PasswordHasher<User>();

            var result = passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            // CALL GenerateToken HERE
            var authResponse = GenerateToken(user);

            return Ok(authResponse);
        }

        private AuthResponseDto GenerateToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiryMinutes = Convert.ToInt32(
                _configuration["Jwt:ExpiryMinutes"]
            );

            var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.UserId.ToString()
        ),

        new Claim(
            ClaimTypes.Email,
            user.Email
        ),

        new Claim(
            ClaimTypes.Role,
            user.Role
        )
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto
            {
                Token = tokenString,
                Email = user.Email,
                Role = user.Role,
                Expiration = expiration
            };
        }
    }
}