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

namespace DALTWNC_QUIZ.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string key { get; set; }

        public List<QuizCardViewModel> FoundQuizzes { get; set; } = new List<QuizCardViewModel>();

        public List<Subject> FoundSubjects { get; set; } = new List<Subject>();

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            List<int> savedQuizIds = new List<int>();
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                if (username != null)
                {
                    var customer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Username == username);
                    if (customer != null)
                    {
                        savedQuizIds = await _context.SavedQuizzes
                            .Where(s => s.CustomerID == customer.CustomerID)
                            .Select(s => s.QuizID)
                            .ToListAsync();
                    }
                }
            }

            var ratingsData = await _context.Ratings
                .AsNoTracking()
                .GroupBy(r => r.QuizID)
                .Select(g => new {
                    QuizID = g.Key,
                    AverageRating = g.Average(r => r.Stars),
                    TotalReviews = g.Count()
                })
                .ToDictionaryAsync(r => r.QuizID);

            var lowerKey = StringUtils.RemoveAccents(key.ToLower());

            var allSubjects = await _context.Subjects.AsNoTracking().ToListAsync();
            FoundSubjects = allSubjects
                .Where(s => StringUtils.RemoveAccents(s.SubjectName.ToLower()).Contains(lowerKey))
                .OrderBy(s => s.SubjectName)
                .ToList();

            var allQuizzes = await _context.Quizzes
                .AsNoTracking()
                .Include(q => q.Subject)
                .Where(q => q.IsPublic)
                .ToListAsync();

            var filteredQuizzes = allQuizzes
                .Where(q => StringUtils.RemoveAccents(q.QuizTitle.ToLower()).Contains(lowerKey))
                .OrderByDescending(q => q.CreatedDate)
                .ToList();

            FoundQuizzes = filteredQuizzes.Select(q =>
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
                    IsSaved = savedQuizIds.Contains(q.QuizID),
                    ReturnUrl = Url.Page("/Search", new { key = key }),
                    AverageRating = ratingInfo?.AverageRating ?? 0,
                    TotalReviews = ratingInfo?.TotalReviews ?? 0
                };
            }).ToList();
        }
    }
}