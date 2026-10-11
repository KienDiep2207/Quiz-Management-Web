using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; 

namespace DALTWNC_QUIZ.Pages.Customer.Quiz_Result 
{
    [Authorize]
    public class ResultModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResultModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public QuizAttempt? ExamResult { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var customer = await _context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Username == username);

            if (customer == null)
            {
                return Forbid();
            }
            ExamResult = await _context.QuizAttempts
                .Include(e => e.Quiz)
                .FirstOrDefaultAsync(e => e.QuizAttemptID == id && e.CustomerID == customer.CustomerID);
            if (ExamResult == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy kết quả bài thi hoặc bạn không có quyền xem.";
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRetakeAsync(int resultId, int quizId)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var customer = await _context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Username == username);

            if (customer == null)
            {
                return Forbid();
            }
            var oldResult = await _context.QuizAttempts
                .FirstOrDefaultAsync(e => e.QuizAttemptID == resultId && e.CustomerID == customer.CustomerID);

            if (oldResult != null)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM QuizAttemptQuestions WHERE QuizAttemptID = {oldResult.QuizAttemptID}"
                );
                _context.QuizAttempts.Remove(oldResult);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa kết quả cũ. Bạn có thể bắt đầu làm lại.";
            }
            else
            {
                TempData["InfoMessage"] = "Bắt đầu làm bài.";
            }
            return RedirectToPage("/Customer/Exam/Take", new { id = quizId, returnUrl = "/" });
        }

        public string FormatDuration(int totalSeconds)
        {
            if (totalSeconds < 60)
            {
                return $"{totalSeconds} giây";
            }

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            if (seconds == 0)
            {
                return $"{minutes} phút";
            }
            return $"{minutes} phút {seconds} giây";
        }
    }
}