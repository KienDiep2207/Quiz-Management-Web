using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DALTWNC_QUIZ.Pages.Customer.Quiz_Result
{
    [Authorize]
    public class ReviewModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReviewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public QuizAttempt ExamResult { get; set; }
        public DALTWNC_QUIZ.Models.Quiz Quiz { get; set; }
        public List<Question> Questions { get; set; }

        public Dictionary<int, int?> UserAnswersMap { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var customer = await _context.Customers.AsNoTracking()
                                         .FirstOrDefaultAsync(c => c.Username == username);
            if (customer == null) return Forbid();

            ExamResult = await _context.QuizAttempts
                .Include(r => r.Quiz)
                    .ThenInclude(q => q.Subject)
                .Include(r => r.QuizAttemptQuestions)
                    .ThenInclude(qaq => qaq.Question)
                        .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(r => r.QuizAttemptID == id && r.CustomerID == customer.CustomerID);

            if (ExamResult == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy kết quả bài thi này.";
                return RedirectToPage("/Index");
            }

            Quiz = ExamResult.Quiz;
            if (Quiz == null) return NotFound("Không tìm thấy bộ đề gốc.");

            Questions = ExamResult.QuizAttemptQuestions
                .OrderBy(qaq => qaq.QuizAttemptQuestionID)
                .Select(qaq => qaq.Question)
                .ToList();

            UserAnswersMap = ExamResult.QuizAttemptQuestions
                .ToDictionary(qaq => qaq.QuestionID, qaq => qaq.SelectedChoiceID);

            return Page();
        }
    }
}