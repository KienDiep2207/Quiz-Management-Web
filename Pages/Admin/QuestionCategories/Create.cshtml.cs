using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.QuestionCategories
{
    [Authorize(Roles = "A")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DALTWNC_QUIZ.Models.QuestionCategory NewCategory { get; set; } = default!;

        public void OnGet()
        {
            ViewData["ActivePage"] = "Categories";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool isDuplicate = await _context.QuestionCategories
                                .AnyAsync(c => c.CategoryName == NewCategory.CategoryName);

            if (isDuplicate)
            {
                ModelState.AddModelError("NewCategory.CategoryName", "Tên danh mục này đã tồn tại.");
                return Page();
            }

            _context.QuestionCategories.Add(NewCategory);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo danh mục mới thành công!";
            return RedirectToPage("./Index");
        }
    }
}
