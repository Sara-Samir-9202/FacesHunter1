
// Controllers/AuthController.cs

using FacesHunter.Data;
using FacesHunter.DTOs;
using FacesHunter.Models;
using FacesHunter.Services;
using Microsoft.AspNetCore.Mvc;

namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IResetPasswordService _resetService;
        private readonly AppDbContext _context;

        public AuthController(AuthService authService, IResetPasswordService resetService, AppDbContext context)
        {
            _authService = authService;
            _resetService = resetService;
            _context = context;
        }

        // ✅ 1. تسجيل مستخدم جديد
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("جميع الحقول مطلوبة");

            if (await _authService.UserExists(request.Username))
                return BadRequest("اسم المستخدم موجود بالفعل");

            var role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role;

            var user = await _authService.Register(request.Username, request.Email, request.Password, role);

            // ✅ إضافة شخص افتراضي عند تسجيل المستخدم
            var defaultPerson = new Person
            {
                FullName = user.Username + " Person",
                Age = 30,
                Gender = Gender.Male,
                LastSeenLocation = "Auto Generated",
                DateMissing = DateTime.UtcNow,
                ReporterName = user.Username,
                ReporterPhone = user.Email,
                ImagePath = "/images/default.jpg",
                FaceEmbedding = ""
            };

            _context.Persons.Add(defaultPerson);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم التسجيل بنجاح",
                user = new { user.Id, user.Username, user.Email, user.Role }
            });
        }

        // ✅ 2. تسجيل الدخول
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("اسم المستخدم وكلمة المرور مطلوبان");

            var loginResult = await _authService.Login(request.Username, request.Password);
            if (loginResult == null)
                return Unauthorized("بيانات الدخول غير صحيحة");

            return Ok(new
            {
                message = "تم تسجيل الدخول بنجاح",
                token = loginResult.Token,
                role = loginResult.Role
            });
        }

        

        // ✅ 4. تعيين كلمة مرور جديدة باستخدام رمز التحقق
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Code) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("جميع الحقول مطلوبة");

            var result = await _resetService.ResetPasswordAsync(dto.Email, dto.Code, dto.NewPassword);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = "تم تعيين كلمة المرور بنجاح" });
        }
    }
}
