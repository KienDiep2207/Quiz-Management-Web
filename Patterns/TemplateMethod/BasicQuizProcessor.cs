using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using System.Linq;

namespace DALTWNC_QUIZ.Patterns.TemplateMethod
{
    public class BasicQuizProcessor : QuizSubmitProcessor
    {
        public BasicQuizProcessor(ApplicationDbContext context) : base(context) { }

        protected override double CalculateScore(QuizAttempt attempt)
        {
            // Tổng số câu hỏi trong bài thi này
            int total = attempt.QuizAttemptQuestions.Count;
            if (total == 0) return 0;

            // Lấy số câu đúng đã được tính ở lớp cha
            int correct = attempt.CorrectAnswers ?? 0;

            // Tính thang điểm 10
            double score = ((double)correct / total) * 10;
            return Math.Round(score, 2);
        }
    }
}