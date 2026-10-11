using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using DALTWNC_QUIZ.Models.ViewModels;
using DALTWNC_QUIZ.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DALTWNC_QUIZ.Patterns.Creational;

namespace DALTWNC_QUIZ.Pages
{
    public class ToggleSaveAjaxModel
    {
        public int QuizId { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly AppConfigurationManager _configManager;

        public IndexModel(ApplicationDbContext context, AppConfigurationManager configManager)
        {
            _context = context;
            _configManager = configManager;
        }

        public List<QuizCardViewModel> QuizList { get; set; } = new List<QuizCardViewModel>();
        public List<int> SavedQuizIds { get; set; } = new List<int>();
        public List<Subject> SubjectList { get; set; } = new List<Subject>();

        public async Task OnGetAsync()
        {
            ViewData["SystemVersion"] = _configManager.SystemVersion;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                if (username != null)
                {
                    var customer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Username == username);

                    if (customer != null)
                    {
                        SavedQuizIds = await _context.SavedQuizzes
                            .Where(s => s.CustomerID == customer.CustomerID)
                            .Select(s => s.QuizID)
                            .ToListAsync();
                    }
                }
            }

            var ratingsData = await _context.Ratings
                                        .AsNoTracking()
                                        .GroupBy(r => r.QuizID)
                                        .Select(g => new
                                        {
                                            QuizID = g.Key,
                                            AverageRating = g.Average(r => r.Stars),
                                            TotalReviews = g.Count()
                                        })
                                        .ToDictionaryAsync(r => r.QuizID);

            var quizzes = await _context.Quizzes
                                        .AsNoTracking()
                                        .Include(q => q.Subject)
                                        .Where(q => q.IsPublic)
                                        .OrderByDescending(q => q.CreatedDate)
                                        .Take(_configManager.MaxFeaturedQuizzes)
                                        .ToListAsync();

            QuizList = quizzes.Select(q =>
            {
                var ratingInfo = ratingsData.GetValueOrDefault(q.QuizID);
                return new QuizCardViewModel
                {
                    QuizID = q.QuizID,
                    QuizTitle = q.QuizTitle,
                    Description = q.Description,
                    SubjectName = q.Subject?.SubjectName ?? "Chung",
                    QuestionCount = q.TotalQuestions,
                    IsDynamic = q.IsDynamic,
                    IsSaved = SavedQuizIds.Contains(q.QuizID),
                    ReturnUrl = Url.Page("/Index"),
                    AverageRating = ratingInfo?.AverageRating ?? 0,
                    TotalReviews = ratingInfo?.TotalReviews ?? 0
                };
            }).ToList();

            SubjectList = await _context.Subjects
                                    .AsNoTracking()
                                    .OrderBy(s => s.SubjectName)
                                    .ToListAsync();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostToggleSaveAsync([FromBody] ToggleSaveAjaxModel model)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return new JsonResult(new { success = false, message = "Bạn cần đăng nhập để lưu bộ đề." });
            }

            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
            {
                return new JsonResult(new { success = false, message = "Lỗi xác thực người dùng." });
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == username);

            if (customer == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy hồ sơ người dùng." });
            }

            var quiz = await _context.Quizzes.FindAsync(model.QuizId);
            if (quiz == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy bộ đề." });
            }

            var saved = await _context.SavedQuizzes
                .FirstOrDefaultAsync(s => s.CustomerID == customer.CustomerID && s.QuizID == model.QuizId);

            bool isCurrentlySaved = false;
            if (saved != null)
            {
                _context.SavedQuizzes.Remove(saved);
                isCurrentlySaved = false;
            }
            else
            {
                _context.SavedQuizzes.Add(new SavedQuiz
                {
                    CustomerID = customer.CustomerID,
                    QuizID = model.QuizId,
                    SavedDate = System.DateTime.UtcNow
                });
                isCurrentlySaved = true;
            }

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, saved = isCurrentlySaved });
        }

        public async Task<IActionResult> OnGetSuggestionsAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length < 2)
            {
                return new JsonResult(new List<string>());
            }

            var lowerKey = StringUtils.RemoveAccents(key.ToLower());

            var allSubjects = await _context.Subjects.AsNoTracking().ToListAsync();
            var subjectSuggestions = allSubjects
                .Where(s => StringUtils.RemoveAccents(s.SubjectName.ToLower()).Contains(lowerKey))
                .Select(s => s.SubjectName)
                .Distinct()
                .Take(3)
                .ToList();

            var allQuizzes = await _context.Quizzes.AsNoTracking().Where(q => q.IsPublic).ToListAsync();
            var quizSuggestions = allQuizzes
                .Where(q => StringUtils.RemoveAccents(q.QuizTitle.ToLower()).Contains(lowerKey))
                .Select(q => q.QuizTitle)
                .Distinct()
                .Take(5)
                .ToList();

            var suggestions = subjectSuggestions.Concat(quizSuggestions)
                                                .Distinct()
                                                .OrderBy(s => s)
                                                .Take(5)
                                                .ToList();

            return new JsonResult(suggestions);
        }


        [ValidateAntiForgeryToken]
        public IActionResult OnPostCheckLogin()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Bạn cần đăng nhập để bắt đầu làm bài thi."
                });
            }

            return new JsonResult(new { success = true });
        }

    }
}