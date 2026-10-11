using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DALTWNC_QUIZ.Pages.Admin.Quizzes
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context) => _context = context;

        public IList<Quiz> QuizList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? query { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Quizzes";

            var quizQuery = _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.User)
                .Include(q => q.QuizQuestions)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
            {
                quizQuery = quizQuery.Where(q =>
                    q.QuizTitle.Contains(query) ||
                    q.Subject.SubjectName.Contains(query));
            }

            QuizList = await quizQuery
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        
            if (!QuizList.Any() && !string.IsNullOrWhiteSpace(query))
            {
                TempData["NoResults"] = true;
            } 
        }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var hasResults = await _context.QuizAttempts.AnyAsync(r => r.QuizID == id);
            if (hasResults)
            {
                TempData["ErrorMessage"] = "Không thể xóa! Bộ đề này đã có người làm bài.";
                return RedirectToPage("./Index");
            }

            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var qqToDelete = await _context.QuizQuestions.Where(qq => qq.QuizID == id).ToListAsync();
                    if (qqToDelete.Any()) _context.QuizQuestions.RemoveRange(qqToDelete);

                    var ratingsToDelete = await _context.Ratings.Where(r => r.QuizID == id).ToListAsync();
                    if (ratingsToDelete.Any()) _context.Ratings.RemoveRange(ratingsToDelete);

                    var commentsToDelete = await _context.Comments.Where(c => c.QuizID == id).ToListAsync();
                    if (commentsToDelete.Any()) _context.Comments.RemoveRange(commentsToDelete);

                    var savedToDelete = await _context.SavedQuizzes.Where(s => s.QuizID == id).ToListAsync();
                    if (savedToDelete.Any()) _context.SavedQuizzes.RemoveRange(savedToDelete);

                    _context.Quizzes.Remove(quiz);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Xóa bộ đề thành công!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa: " + ex.Message;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}