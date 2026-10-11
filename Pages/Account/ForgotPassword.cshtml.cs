using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DALTWNC_QUIZ.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ForgotPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ForgotInput Input { get; set; } = new();

        public class ForgotInput
        {
            [Required(ErrorMessage = "Vui lòng nhập email hoặc tên đăng nhập.")]
            public string EmailOrUsername { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp!")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            if (TempData["SuccessMessage"] != null)
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            else if (TempData["ErrorMessage"] != null)
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ hoặc xác nhận mật khẩu không khớp!";
                return RedirectToPage();
            }

            var input = Input.EmailOrUsername.Trim();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerEmail == input || c.Username == input);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản với thông tin đã nhập!";
                return RedirectToPage();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == customer.Username);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản tương ứng trong hệ thống!";
                return RedirectToPage();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(Input.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Hãy đăng nhập lại.";
            return RedirectToPage("Login");
        }
    }
}
