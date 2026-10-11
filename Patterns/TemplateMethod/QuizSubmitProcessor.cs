using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Patterns.TemplateMethod
{
    public abstract class QuizSubmitProcessor
    {
        protected readonly ApplicationDbContext _context;

        public QuizSubmitProcessor(ApplicationDbContext context)
        {
            _context = context;
        }

        // ĐÂY LÀ TEMPLATE METHOD (Khung thuật toán)
        public async Task<QuizAttempt> SubmitQuizAsync(int attemptId)
        {
            // 1. Lấy dữ liệu (Dùng đúng QuizAttemptID từ Model của bạn)
            var attempt = await GetAttemptWithAnswersAsync(attemptId);
            if (attempt == null) throw new Exception("Không tìm thấy bài thi.");

            // 2. Kiểm tra tính hợp lệ
            ValidateAttempt(attempt);

            // 3. Đánh dấu đã hoàn thành (Khớp với trường IsCompleted trong Model)
            attempt.IsCompleted = true;

            // 4. Tính toán số câu đúng (Hàm bổ trợ bên dưới)
            attempt.CorrectAnswers = CountCorrectAnswers(attempt);

            // 5. Tính điểm (Bước trừu tượng để lớp con thực hiện)
            double calculatedScore = CalculateScore(attempt);
            attempt.Score = (decimal?)calculatedScore;

            // 6. Các bước Hook và Lưu
            OnBeforeSave(attempt);
            await SaveResultAsync(attempt);
            PostProcessing(attempt);

            return attempt;
        }

        // --- Các hàm bổ trợ (Private/Protected) ---

        private async Task<QuizAttempt> GetAttemptWithAnswersAsync(int attemptId)
        {
            return await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.QuizAttemptQuestions)
                    .ThenInclude(qaq => qaq.Question)
                        .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(a => a.QuizAttemptID == attemptId);
        }

        private void ValidateAttempt(QuizAttempt attempt)
        {
            if (attempt.IsCompleted)
            {
                throw new Exception("Bài thi này đã được hoàn thành trước đó.");
            }
        }

        // Hàm tính số câu đúng dựa trên Model thực tế của bạn
        private int CountCorrectAnswers(QuizAttempt attempt)
        {
            return attempt.QuizAttemptQuestions.Count(qaq =>
                qaq.Question.Choices.Any(c => c.IsCorrect && c.ChoiceID == qaq.SelectedChoiceID));
        }

        private async Task SaveResultAsync(QuizAttempt attempt)
        {
            _context.QuizAttempts.Update(attempt);
            await _context.SaveChangesAsync();
        }

        // --- Các phương thức để lớp con ghi đè ---

        protected abstract double CalculateScore(QuizAttempt attempt);

        protected virtual void OnBeforeSave(QuizAttempt attempt) { }

        protected virtual void PostProcessing(QuizAttempt attempt) { }
    }
}