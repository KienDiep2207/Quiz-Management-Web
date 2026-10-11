using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Customer.Exam
{
    public class TakeModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly DALTWNC_QUIZ.Patterns.TemplateMethod.BasicQuizProcessor _quizProcessor;

        public TakeModel(ApplicationDbContext context, DALTWNC_QUIZ.Patterns.TemplateMethod.BasicQuizProcessor quizProcessor)
        {
            _context = context;
            _quizProcessor = quizProcessor; // Gán vào biến dùng chung
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public DALTWNC_QUIZ.Models.Quiz Quiz { get; set; } = default!;
        public List<Question> Questions { get; set; } = new();

        public int Duration { get; set; }
        public int RemainingSeconds { get; set; }
        public int CurrentAttemptID { get; set; }

        public bool IsExamStarted { get; set; } = false;

        [BindProperty]
        public Dictionary<string, string> Answers { get; set; } = new();

        [BindProperty]
        public int ElapsedSeconds { get; set; }

        private int CalculateDuration(int totalQuestions)
        {
            int minutes = (int)Math.Ceiling(totalQuestions * 1.5);
            return minutes > 0 ? minutes : 15;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Account/Login");

            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Username == username);
            if (customer == null) return RedirectToPage("/Index");

            Quiz = await _context.Quizzes.AsNoTracking().FirstOrDefaultAsync(q => q.QuizID == Id);
            if (Quiz == null) return NotFound();

            Duration = CalculateDuration(Quiz.TotalQuestions);

            var existingAttempt = await _context.QuizAttempts
                .Include(qa => qa.QuizAttemptQuestions).ThenInclude(qaq => qaq.Question).ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(qa => qa.QuizID == Id && qa.CustomerID == customer.CustomerID && qa.IsCompleted == false);

            if (existingAttempt != null)
            {
                IsExamStarted = true;
                CurrentAttemptID = existingAttempt.QuizAttemptID;

                Questions = existingAttempt.QuizAttemptQuestions
                                .OrderBy(q => q.QuizAttemptQuestionID)
                                .Select(q => q.Question)
                                .ToList();

                var timePassed = DateTime.UtcNow - existingAttempt.StartTime;
                int totalSecondsAllowed = Duration * 60;
                RemainingSeconds = totalSecondsAllowed - (int)timePassed.TotalSeconds;

                if (RemainingSeconds < 0) RemainingSeconds = 0;
            }
            else
            {
                IsExamStarted = false;
                RemainingSeconds = Duration * 60;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostStartAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.QuizID == Id);

            if (customer == null || quiz == null) return RedirectToPage("/Index");

            List<int> questionIds;
            if (quiz.IsDynamic)
            {
                var random = new Random();
                var allIds = await _context.Questions.Where(q => q.SubjectID == quiz.SubjectID).Select(q => q.QuestionID).ToListAsync();
                int count = Math.Min(quiz.TotalQuestions, allIds.Count);
                questionIds = allIds.OrderBy(x => random.Next()).Take(count).ToList();
            }
            else
            {
                questionIds = await _context.QuizQuestions.Where(qq => qq.QuizID == Id).OrderBy(qq => qq.QuestionID).Select(qq => qq.QuestionID).ToListAsync();
            }

            var attempt = new QuizAttempt
            {
                CustomerID = customer.CustomerID,
                QuizID = Id,
                StartTime = DateTime.UtcNow,
                IsCompleted = false,
                TotalQuestions = questionIds.Count
            };
            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            var attemptQuestions = questionIds.Select(qId => new QuizAttemptQuestion
            {
                QuizAttemptID = attempt.QuizAttemptID,
                QuestionID = qId
            }).ToList();

            _context.QuizAttemptQuestions.AddRange(attemptQuestions);
            await _context.SaveChangesAsync();

            return RedirectToPage("Take", new { id = Id });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Lấy ID từ Form (Giữ nguyên logic của bạn)
            var currentAttemptId = int.Parse(Request.Form["CurrentAttemptID"]);
            int.TryParse(Request.Form["ElapsedSeconds"], out int elapsedSecondsFromForm);

            // 2. Tìm bài thi (Giữ nguyên để kiểm tra tồn tại)
            var attempt = await _context.QuizAttempts
                .Include(qa => qa.QuizAttemptQuestions)
                .FirstOrDefaultAsync(qa => qa.QuizAttemptID == currentAttemptId && qa.IsCompleted == false);

            if (attempt == null) return RedirectToPage("/Index");

            // 3. LƯU CÁC CÂU TRẢ LỜI CỦA NGƯỜI DÙNG VÀO DATABASE TRƯỚC
            // Bước này quan trọng vì Template Method sẽ đọc dữ liệu từ DB để tính điểm
            foreach (var attemptQuestion in attempt.QuizAttemptQuestions)
            {
                var qId = attemptQuestion.QuestionID.ToString();
                if (Answers.TryGetValue(qId, out string choiceIdStr) && int.TryParse(choiceIdStr, out int choiceId))
                {
                    attemptQuestion.SelectedChoiceID = choiceId;
                }
            }

            // Cập nhật thời gian làm bài vào Model trước khi tính điểm
            attempt.DurationMinute = elapsedSecondsFromForm;

            await _context.SaveChangesAsync();

            // 4. GỌI TEMPLATE METHOD ĐỂ XỬ LÝ (TÍNH ĐIỂM, HOÀN THÀNH, LƯU)
            try
            {
                // Thay vì viết logic tính toán ở đây, ta gọi "cỗ máy" đã thiết kế
                await _quizProcessor.SubmitQuizAsync(currentAttemptId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi xử lý bài thi: " + ex.Message);
                return Page();
            }

            TempData["SuccessMessage"] = "Bạn đã nộp bài thành công!";
            return RedirectToPage("/Customer/Quiz_Result/Result", new { id = currentAttemptId });
        }
    }
}