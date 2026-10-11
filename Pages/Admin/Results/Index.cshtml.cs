using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.Results
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<QuizAttempt> ResultList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Results";
            ViewData["CurrentQuery"] = query;

            var resultQuery = _context.QuizAttempts
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .Include(r => r.Quiz)
                .Where(r => r.IsCompleted == true)
                .OrderByDescending(r => r.StartTime)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query))
            {
                resultQuery = resultQuery.Where(r =>
                    (r.Customer != null && r.Customer.CustomerName.Contains(query)) ||
                    (r.Quiz != null && r.Quiz.QuizTitle.Contains(query))
                );
            }

            ResultList = await resultQuery.ToListAsync();
        }
    }
}