using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using BCrypt.Net;

namespace DALTWNC_QUIZ.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new RegisterInput();

        public class RegisterInput
        {
            [Required] public string Username { get; set; } = string.Empty;
            [Required] public string CustomerName { get; set; } = string.Empty;
            [Required] public string CustomerPhone { get; set; } = string.Empty;
            public string? CustomerEmail { get; set; }
            public string? CustomerAddress { get; set; }
            [Required] public string Password { get; set; } = string.Empty; 
            [Required][Compare("Password", ErrorMessage = "Mật khẩu không khớp!")] public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var existingUser = await _context.Users.FindAsync(Input.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Input.Username", "Tên đăng nhập đã tồn tại.");
                return Page();
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.Password);
            var user = new User
            {
                Username = Input.Username,
                Password = hashedPassword,
                UserRole = "C",
                IsLocked = false
            };

            var customer = new DALTWNC_QUIZ.Models.Customer
            {
                CustomerName = Input.CustomerName,
                CustomerPhone = Input.CustomerPhone,
                CustomerEmail = Input.CustomerEmail,
                CustomerAddress = Input.CustomerAddress,
                Username = Input.Username
            };

            _context.Users.Add(user);
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đăng ký thành công! Hãy đăng nhập.";
            return RedirectToPage("Login");
        }
    }
}