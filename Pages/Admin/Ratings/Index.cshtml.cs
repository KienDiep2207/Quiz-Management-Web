using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.Ratings
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Rating> RatingList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Ratings";
            ViewData["CurrentQuery"] = query;

            var ratingQuery = _context.Ratings
                .Include(r => r.Customer)
                .Include(r => r.Quiz)
                .OrderByDescending(r => r.RatingDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query))
            {
                ratingQuery = ratingQuery.Where(r =>
                    (r.Customer != null && r.Customer.CustomerName.Contains(query)) ||
                    (r.Quiz != null && r.Quiz.QuizTitle.Contains(query)) ||
                    (r.Comment != null && r.Comment.Contains(query))
                );
            }

            RatingList = await ratingQuery.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            try
            {
                _context.Ratings.Remove(rating);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa đánh giá thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa.";
            }

            return RedirectToPage("./Index");
        }
    }
}