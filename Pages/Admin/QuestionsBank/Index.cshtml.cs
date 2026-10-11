using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DALTWNC_QUIZ.Pages.Admin.QuestionsBank
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context) => _context = context;

        public IList<Question> QuestionList { get; set; } = default!;
        public SelectList SubjectSL { get; set; }

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SubjectFilterId { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "QuestionsBank";
            ViewData["CurrentQuery"] = query;

            SubjectSL = new SelectList(await _context.Subjects.AsNoTracking().ToListAsync(), "SubjectID", "SubjectName", SubjectFilterId);

            var questionQuery = _context.Questions
                .Include(q => q.Subject)
                .Include(q => q.QuestionCategory)
                .Include(q => q.Choices)
                .AsNoTracking();

            if (SubjectFilterId != 0)
            {
                questionQuery = questionQuery.Where(q => q.SubjectID == SubjectFilterId);
            }

            if (!string.IsNullOrEmpty(query))
            {
                questionQuery = questionQuery.Where(q => q.QuestionText.Contains(query));
            }

            QuestionList = await questionQuery
                .OrderByDescending(q => q.QuestionID)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var question = await _context.Questions.Include(q => q.Choices).FirstOrDefaultAsync(q => q.QuestionID == id);

            if (question == null) return NotFound();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var isUsedInQuiz = await _context.QuizQuestions.AnyAsync(qq => qq.QuestionID == id);
                    if (isUsedInQuiz)
                    {
                        TempData["ErrorMessage"] = "Không thể xóa! Câu hỏi này đang được sử dụng trong một hoặc nhiều bộ đề.";
                        return RedirectToPage("./Index", new { query = query, subjectFilterId = SubjectFilterId });
                    }

                    _context.Choices.RemoveRange(question.Choices);

                    _context.Questions.Remove(question);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Xóa câu hỏi thành công!";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa câu hỏi.";
                    await transaction.RollbackAsync();
                }
            }
            return RedirectToPage("./Index", new { query = query, subjectFilterId = SubjectFilterId });
        }
    }
}