using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Admin.Subject
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
        public DALTWNC_QUIZ.Models.Subject Subject { get; set; } = default!;

        public SelectList ParentSubjectSL { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["ActivePage"] = "Subjects";

            if (id == null) return NotFound();

            var subject = await _context.Subjects.FirstOrDefaultAsync(m => m.SubjectID == id);
            if (subject == null) return NotFound();

            Subject = subject;

            var subjectsList = await _context.Subjects
                                    .AsNoTracking()
                                    .Where(s => s.SubjectID != id && s.ParentID == null)
                                    .ToListAsync();

            int selectedValue = Subject.ParentID ?? 0;
            ParentSubjectSL = new SelectList(subjectsList, "SubjectID", "SubjectName", selectedValue);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var subjectsList = await _context.Subjects
                                    .AsNoTracking()
                                    .Where(s => s.SubjectID != Subject.SubjectID && s.ParentID == null)
                                    .ToListAsync();

            if (!ModelState.IsValid)
            {
                ParentSubjectSL = new SelectList(subjectsList, "SubjectID", "SubjectName", Subject.ParentID ?? 0);
                return Page();
            }

            if (Subject.ParentID == 0)
            {
                Subject.ParentID = null;
            }
            bool isDuplicate = await _context.Subjects.AnyAsync(s =>
                s.SubjectName == Subject.SubjectName &&
                s.SubjectID != Subject.SubjectID);

            if (isDuplicate)
            {
                ModelState.AddModelError("Subject.SubjectName", "Tên môn học này đã tồn tại.");
                ParentSubjectSL = new SelectList(subjectsList, "SubjectID", "SubjectName", Subject.ParentID ?? 0);
                return Page();
            }
            _context.Attach(Subject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(Subject.SubjectID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["SuccessMessage"] = "Cập nhật môn học thành công!";
            return RedirectToPage("./Index");
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.SubjectID == id);
        }
    }
}