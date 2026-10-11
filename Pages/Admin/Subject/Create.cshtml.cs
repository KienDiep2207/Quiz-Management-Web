using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DALTWNC_QUIZ.Pages.Admin.Subject
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
        public DALTWNC_QUIZ.Models.Subject NewSubject { get; set; } = new();

        public SelectList ParentSubjectSL { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Subjects";

            var subjects = await _context.Subjects
                                    .Where(s => s.ParentID == null)
                                    .AsNoTracking()
                                    .ToListAsync();

            ParentSubjectSL = new SelectList(subjects, "SubjectID", "SubjectName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            if (NewSubject.ParentID == 0)
            {
                NewSubject.ParentID = null;
            }
            try
            {
                _context.Subjects.Add(NewSubject);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm môn học thành công!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu: " + ex.Message);
                await OnGetAsync();
                return Page();
            }
        }
    }
}