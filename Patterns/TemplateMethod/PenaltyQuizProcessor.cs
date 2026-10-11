using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;

namespace DALTWNC_QUIZ.Patterns.TemplateMethod
{
    public class PenaltyQuizProcessor : QuizSubmitProcessor
    {
        public PenaltyQuizProcessor(ApplicationDbContext context) : base(context) { }

        protected override double CalculateScore(QuizAttempt attempt)
        {
            double finalScore = 0;
            double pointsPerCorrect = 1.0;
            double penaltyPerWrong = 0.25; // Sai một câu trừ 0.25 điểm

            foreach (var qaq in attempt.QuizAttemptQuestions)
            {
                var correctChoice = qaq.Question.Choices.FirstOrDefault(c => c.IsCorrect);

                if (correctChoice != null && qaq.SelectedChoiceID == correctChoice.ChoiceID)
                {
                    finalScore += pointsPerCorrect;
                }
                else if (qaq.SelectedChoiceID != null) // Nếu có chọn nhưng sai
                {
                    finalScore -= penaltyPerWrong;
                }
            }

            return finalScore < 0 ? 0 : finalScore; // Không để điểm âm
        }
    }
}