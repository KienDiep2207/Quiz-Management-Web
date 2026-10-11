namespace DALTWNC_QUIZ.Patterns.Creational
{
    public class AppConfigurationManager
    {
        public string SystemVersion { get; private set; }
        public int MaxFeaturedQuizzes { get; private set; }

        public AppConfigurationManager()
        {
            SystemVersion = "Phiên bản 1.0 (Áp dụng Design Patterns)";
            MaxFeaturedQuizzes = 6; 
        }
    }
}