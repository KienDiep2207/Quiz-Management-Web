using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.Subject
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<DALTWNC_QUIZ.Models.Subject> SubjectList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Subjects";
            ViewData["CurrentQuery"] = query;

            var subjectQuery = _context.Subjects
                                    .Include(s => s.ParentSubject)
                                    .AsNoTracking();

            if (!string.IsNullOrEmpty(query))
            {
                subjectQuery = subjectQuery.Where(s => s.SubjectName.Contains(query));
            }

            SubjectList = await subjectQuery.ToListAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            var hasChildSubjects = await _context.Subjects.AnyAsync(s => s.ParentID == id);
            if (hasChildSubjects)
            {
                TempData["ErrorMessage"] = "Không thể xóa! Môn học này đang là môn cha của môn học khác.";
                return RedirectToPage("./Index");
            }

            var hasQuestions = await _context.Questions.AnyAsync(q => q.SubjectID == id);
            if (hasQuestions)
            {
                TempData["ErrorMessage"] = "Không thể xóa! Môn học này đã chứa bộ đề thi.";
                return RedirectToPage("./Index");
            }

            try
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa môn học thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình xóa.";
            }

            return RedirectToPage("./Index");
        }
    }
}