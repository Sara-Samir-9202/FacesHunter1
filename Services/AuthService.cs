
// Services/AuthService.cs

using FacesHunter.Data;
using FacesHunter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FacesHunter.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly string _jwtKey;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

            _jwtKey = _config["Jwt:Key"] ?? throw new Exception("JWT Key is missing from configuration");

            if (_jwtKey.Length < 64)
                throw new Exception("JWT Key must be at least 64 characters long for HS512");
        }

        // ✅ تحقق من وجود المستخدم
        public async Task<bool> UserExists(string username)
        {
            return !string.IsNullOrWhiteSpace(username) &&
                   await _context.Users.AnyAsync(u => u.Username == username);
        }

        // ✅ إنشاء Hash و Salt
        public void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password!));
        }

        // ✅ التحقق من تطابق كلمة السر
        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password) || storedHash == null || storedSalt == null)
                return false;

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password!));
            return computedHash.SequenceEqual(storedHash);
        }

        // ✅ تسجيل مستخدم جديد
        public async Task<User> Register(string username, string email, string password, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            CreatePasswordHash(password, out byte[] hash, out byte[] salt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // ✅ تسجيل الدخول وإرجاع JWT Token + Role
        public async Task<LoginResult?> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || user.PasswordHash == null || user.PasswordSalt == null)
                return null;

            var isValid = VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
                return null;

            var token = GenerateToken(user);

            return new LoginResult
            {
                Token = token,
                Role = user.Role ?? "User"
            };
        }

        // ✅ إنشاء JWT Token
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
