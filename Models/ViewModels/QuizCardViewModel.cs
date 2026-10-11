namespace DALTWNC_QUIZ.Models.ViewModels
{
    public class QuizCardViewModel
    {
        public int QuizID { get; set; }
        public string QuizTitle { get; set; }
        public string Description { get; set; }
        public string SubjectName { get; set; }
        public int QuestionCount { get; set; }
        public bool IsSaved { get; set; } 
        public string ReturnUrl { get; set; }

        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public bool IsDynamic { get; set; }

    }
}
