using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.QuestionCategories
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<DALTWNC_QUIZ.Models.QuestionCategory> CategoryList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "QuestionCategories";
            ViewData["CurrentQuery"] = query;

            var categoryQuery = _context.QuestionCategories.AsNoTracking();

            if (!string.IsNullOrEmpty(query))
            {
                categoryQuery = categoryQuery.Where(c => c.CategoryName.Contains(query));
            }

            CategoryList = await categoryQuery.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var category = await _context.QuestionCategories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var hasQuestions = await _context.Questions.AnyAsync(q => q.CategoryID == id);
            if (hasQuestions)
            {
                TempData["ErrorMessage"] = "Không thể xóa! Danh mục này đã chứa câu hỏi.";
                return RedirectToPage("./Index");
            }

            try
            {
                _context.QuestionCategories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa.";
            }

            return RedirectToPage("./Index");
        }
    }
}