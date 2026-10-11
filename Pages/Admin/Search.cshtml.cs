using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DALTWNC_QUIZ.Utils;

public class SuggestionModel
{
    public string Label { get; set; }
    public string Category { get; set; }
    public string Url { get; set; }
}

namespace DALTWNC_QUIZ.Pages.Admin
{
    [Authorize(Roles = "A")]
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        public List<DALTWNC_QUIZ.Models.User> FoundUsers { get; set; } = new();
        public List<DALTWNC_QUIZ.Models.Subject> FoundSubjects { get; set; } = new();
        public List<Quiz> FoundQuizzes { get; set; } = new();
        public List<QuestionCategory> FoundCategories { get; set; } = new();
        public List<Question> FoundQuestions { get; set; } = new();
        public List<QuizAttempt> FoundResults { get; set; } = new();
        public List<Comment> FoundComments { get; set; } = new();
        public List<Rating> FoundRatings { get; set; } = new();

        public List<string> ResultOrder { get; set; } = new();

        public async Task<IActionResult> OnGetSuggestionsAsync(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return new JsonResult(new List<SuggestionModel>());
            }

            var suggestions = new List<SuggestionModel>();

            string normalizedTerm = term.ToLower();
            string accentInsensitiveTerm = StringUtils.RemoveAccents(normalizedTerm);

            var subjects = (await _context.Subjects.ToListAsync())
                .Where(s => StringUtils.RemoveAccents(s.SubjectName.ToLower()).Contains(accentInsensitiveTerm))
                .Select(s => new SuggestionModel
                {
                    Label = s.SubjectName,
                    Category = "Môn học",
                    Url = Url.Page("/Admin/Subject/Edit", new { id = s.SubjectID })
                })
                .Take(5)
                .ToList();
            suggestions.AddRange(subjects);

            var quizzes = (await _context.Quizzes.ToListAsync())
                .Where(q => StringUtils.RemoveAccents(q.QuizTitle.ToLower()).Contains(accentInsensitiveTerm))
                .Select(q => new SuggestionModel
                {
                    Label = q.QuizTitle,
                    Category = "Bộ đề",
                    Url = Url.Page("/Admin/Quizzes/Edit", new { id = q.QuizID })
                })
                .Take(5)
                .ToList();
            suggestions.AddRange(quizzes);

            var questions = (await _context.Questions.ToListAsync())
                .Where(q => StringUtils.RemoveAccents(q.QuestionText.ToLower()).Contains(accentInsensitiveTerm))
                .Select(q => new SuggestionModel
                {
                    Label = q.QuestionText.Length > 70 ? q.QuestionText.Substring(0, 70) + "..." : q.QuestionText,
                    Category = "Câu hỏi",
                    Url = Url.Page("/Admin/QuestionsBank/Edit", new { id = q.QuestionID })
                })
                .Take(5)
                .ToList();
            suggestions.AddRange(questions);

            var users = (await _context.Users.ToListAsync())
                .Where(u => StringUtils.RemoveAccents(u.Username.ToLower()).Contains(accentInsensitiveTerm))
                .Select(u => new SuggestionModel
                {
                    Label = u.Username,
                    Category = "User",
                    Url = Url.Page("/Admin/User/Edit", new { id = u.Username })
                })
                .Take(5)
                .ToList();
            suggestions.AddRange(users);

            return new JsonResult(suggestions.OrderBy(s => s.Category));
        }

        public async Task OnGetAsync()
        {
            ViewData["CurrentQuery"] = query;
            ViewData["ActivePage"] = "";

            if (!string.IsNullOrEmpty(query))
            {
                FoundUsers = await _context.Users
                    .Include(u => u.Customer)
                    .Where(u => u.Username.Contains(query) || (u.Customer != null && u.Customer.CustomerName.Contains(query)))
                    .Take(10).ToListAsync();

                FoundSubjects = await _context.Subjects
                    .Where(s => s.SubjectName.Contains(query))
                    .Take(10).ToListAsync();

                FoundQuizzes = await _context.Quizzes
                    .Where(q => q.QuizTitle.Contains(query))
                    .Take(10).ToListAsync();

                FoundCategories = await _context.QuestionCategories
                    .Where(c => c.CategoryName.Contains(query))
                    .Take(10).ToListAsync();

                FoundQuestions = await _context.Questions
                    .Include(q => q.Subject)
                    .Where(q => q.QuestionText.Contains(query))
                    .Take(10).ToListAsync();

                FoundResults = await _context.QuizAttempts
                    .Include(r => r.Customer)
                    .Include(r => r.Quiz)
                    .Where(r => (r.Customer != null && r.Customer.CustomerName.Contains(query)) ||
                                (r.Quiz != null && r.Quiz.QuizTitle.Contains(query)))
                    .Take(10).ToListAsync();

                FoundComments = await _context.Comments
                    .Include(c => c.Customer)
                    .Include(c => c.Quiz)
                    .Where(c => c.Content.Contains(query) ||
                                (c.Customer != null && c.Customer.CustomerName.Contains(query)) ||
                                (c.Quiz != null && c.Quiz.QuizTitle.Contains(query)))
                    .Take(10).ToListAsync();

                FoundRatings = await _context.Ratings
                    .Include(r => r.Customer)
                    .Include(r => r.Quiz)
                     .Where(r => (r.Comment != null && r.Comment.Contains(query)) ||
                                 (r.Customer != null && r.Customer.CustomerName.Contains(query)) ||
                                 (r.Quiz != null && r.Quiz.QuizTitle.Contains(query)))
                    .Take(10).ToListAsync();

                if (FoundUsers.Any()) ResultOrder.Add("Users");
                if (FoundSubjects.Any()) ResultOrder.Add("Subjects");
                if (FoundQuizzes.Any()) ResultOrder.Add("Quizzes");
                if (FoundCategories.Any()) ResultOrder.Add("Categories");
                if (FoundQuestions.Any()) ResultOrder.Add("Questions");
                if (FoundResults.Any()) ResultOrder.Add("Results");
                if (FoundComments.Any()) ResultOrder.Add("Comments");
                if (FoundRatings.Any()) ResultOrder.Add("Ratings");

                if (!FoundUsers.Any()) ResultOrder.Add("Users");
                if (!FoundSubjects.Any()) ResultOrder.Add("Subjects");
                if (!FoundQuizzes.Any()) ResultOrder.Add("Quizzes");
                if (!FoundCategories.Any()) ResultOrder.Add("Categories");
                if (!FoundQuestions.Any()) ResultOrder.Add("Questions");
                if (!FoundResults.Any()) ResultOrder.Add("Results");
                if (!FoundComments.Any()) ResultOrder.Add("Comments");
                if (!FoundRatings.Any()) ResultOrder.Add("Ratings");

            }
        }
    }
}