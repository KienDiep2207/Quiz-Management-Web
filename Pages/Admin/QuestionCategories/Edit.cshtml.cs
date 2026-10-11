using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.QuestionCategories
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
        public DALTWNC_QUIZ.Models.QuestionCategory CategoryToEdit { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ViewData["ActivePage"] = "Categories";

            var category = await _context.QuestionCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            CategoryToEdit = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool isDuplicate = await _context.QuestionCategories
                .AnyAsync(c => c.CategoryName == CategoryToEdit.CategoryName &&
                               c.CategoryID != CategoryToEdit.CategoryID);

            if (isDuplicate)
            {
                ModelState.AddModelError("CategoryToEdit.CategoryName", "Tên danh mục này đã tồn tại.");
                return Page();
            }

            _context.Attach(CategoryToEdit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.QuestionCategories.AnyAsync(c => c.CategoryID == CategoryToEdit.CategoryID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            return RedirectToPage("./Index");
        }
    }
}