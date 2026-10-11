using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.User
{
    [Authorize(Roles = "A")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DALTWNC_QUIZ.Models.User UserToEdit { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            ViewData["ActivePage"] = "Users";
            if (id == null) return NotFound();

            UserToEdit = await _context.Users.FindAsync(id);

            if (UserToEdit == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var userFromDb = await _context.Users.FindAsync(UserToEdit.Username);
            if (userFromDb == null) return NotFound();

            userFromDb.UserRole = UserToEdit.UserRole;
            userFromDb.IsLocked = UserToEdit.IsLocked;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin User thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Lỗi! Không thể cập nhật.";
            }

            return RedirectToPage("./Index");
        }
    }
}
