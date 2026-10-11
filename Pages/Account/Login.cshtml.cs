using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DALTWNC_QUIZ.Data;
using Microsoft.EntityFrameworkCore;

using BCrypt.Net;

namespace DALTWNC_QUIZ.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"]!.ToString();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Username == Input.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(Input.Password, user.Password))
            {
                ViewData["Error"] = "Sai tài khoản hoặc mật khẩu!";
                return Page();
            }
            if (user.IsLocked)
            {
                ViewData["Error"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,            
                AllowRefresh = false,             
                ExpiresUtc = null                 
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return user.UserRole switch
            {
                "A" => RedirectToPage("/Admin/Home"),
                "C" => RedirectToPage("/Index"),
                _ => RedirectToPage("/Index")
            };
        }
    }
}
