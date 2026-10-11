using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Shared.Components.RatingSummary
{
    public class RatingSummaryViewModel
    {
        public int QuizId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public double FiveStarPercent { get; set; }
        public double FourStarPercent { get; set; }
        public double ThreeStarPercent { get; set; }
        public double TwoStarPercent { get; set; }
        public double OneStarPercent { get; set; }
    }

    public class RatingSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public RatingSummaryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int quizId, int currentUserRating)
        {
            var ratings = await _context.Ratings
                .Where(r => r.QuizID == quizId)
                .AsNoTracking()
                .ToListAsync();

            int totalReviews = ratings.Count;
            double avgRating = 0;
            double fiveStar = 0, fourStar = 0, threeStar = 0, twoStar = 0, oneStar = 0;

            if (totalReviews > 0)
            {
                avgRating = ratings.Average(r => r.Stars);
                fiveStar = (double)ratings.Count(r => r.Stars == 5) / totalReviews * 100;
                fourStar = (double)ratings.Count(r => r.Stars == 4) / totalReviews * 100;
                threeStar = (double)ratings.Count(r => r.Stars == 3) / totalReviews * 100;
                twoStar = (double)ratings.Count(r => r.Stars == 2) / totalReviews * 100;
                oneStar = (double)ratings.Count(r => r.Stars == 1) / totalReviews * 100;
            }

            var viewModel = new RatingSummaryViewModel
            {
                QuizId = quizId,
                AverageRating = avgRating,
                TotalReviews = totalReviews,
                FiveStarPercent = fiveStar,
                FourStarPercent = fourStar,
                ThreeStarPercent = threeStar,
                TwoStarPercent = twoStar,
                OneStarPercent = oneStar
            };

            ViewData["CurrentUserRating"] = currentUserRating;

            return View(viewModel);
        }
    }
}