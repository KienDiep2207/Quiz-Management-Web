using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Customer.Quiz
{
    public class SavedModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SavedModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SavedQuiz> SavedQuizzes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToPage("/Account/Login");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == User.Identity.Name);

            if (customer == null)
                return RedirectToPage("/Account/Login");

            SavedQuizzes = await _context.SavedQuizzes
                .Include(s => s.Quiz)
                    .ThenInclude(q => q.Subject)
                .Where(s => s.CustomerID == customer.CustomerID)
                .OrderByDescending(s => s.SavedDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUnsaveAsync(int id)
        {
            var saved = await _context.SavedQuizzes.FindAsync(id);
            if (saved != null)
            {
                _context.SavedQuizzes.Remove(saved);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã bỏ lưu bộ đề.";
            }

            return RedirectToPage();
        }
    }
}
