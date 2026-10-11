namespace DALTWNC_QUIZ.Models.ViewModels
{
    public class DashboardStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalExamResults { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalCategories { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalCommentsAwaitingApproval { get; set; }

        public string AverageRatingText => $"{AverageRating:F1} / 5";
    }

    public class DashboardCard
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Link { get; set; }

        public DashboardCard(string title, string value, string icon, string color, string link)
        {
            Title = title;
            Value = value;
            Icon = icon;
            Color = color;
            Link = link;
        }
    }
}