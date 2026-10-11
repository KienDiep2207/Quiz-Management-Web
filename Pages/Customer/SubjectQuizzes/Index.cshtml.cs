using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using DALTWNC_QUIZ.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DALTWNC_QUIZ.Pages.Customer.SubjectQuizzes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class QuizViewModel
        {
            public int QuizID { get; set; }
            public string QuizTitle { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string SubjectName { get; set; } = string.Empty;
            public int QuestionCount { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public Subject Subject { get; set; } = default!;
        public IList<QuizCardViewModel> QuizList { get; set; } = new List<QuizCardViewModel>();
        public List<int> SavedQuizIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            Subject = await _context.Subjects.FindAsync(id);
            if (Subject == null)
            {
                return NotFound();
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                if (username != null)
                {
                    var customer = await _context.Customers.AsNoTracking()
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
                                    .Include(q => q.Subject)
                                    .Include(q => q.QuizQuestions)
                                    .Where(q => q.SubjectID == id && q.IsPublic)
                                    .ToListAsync();

            QuizList = quizzes.Select(q =>
            {
                var ratingInfo = ratingsData.GetValueOrDefault(q.QuizID);

                return new QuizCardViewModel
                {
                    QuizID = q.QuizID,
                    QuizTitle = q.QuizTitle,
                    Description = q.Description,
                    SubjectName = q.Subject.SubjectName,
                    QuestionCount = q.TotalQuestions,
                    IsDynamic = q.IsDynamic,
                    IsSaved = SavedQuizIds.Contains(q.QuizID),
                    ReturnUrl = Url.Page("/Customer/SubjectQuizzes/Index", new { id = this.id }),

                    AverageRating = ratingInfo?.AverageRating ?? 0,
                    TotalReviews = ratingInfo?.TotalReviews ?? 0
                };
            }).ToList();

            return Page();
        }
    }
}