using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Models.ViewComponents
{
    public class DashboardStatsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public DashboardStatsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new DashboardStatsViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalQuizzes = await _context.Quizzes.CountAsync(),
                TotalExamResults = await _context.QuizAttempts.CountAsync(),
                TotalSubjects = await _context.Subjects.CountAsync(),
                TotalCategories = await _context.QuestionCategories.CountAsync(),
                TotalQuestions = await _context.Questions.CountAsync(),
                TotalCommentsAwaitingApproval = await _context.Comments.CountAsync(c => c.IsApproved == false),

                AverageRating = await _context.Ratings.AnyAsync()
            ? (decimal)await _context.Ratings.AverageAsync(r => r.Stars)
            : 0.0m
            };

            return View(model);
        }
    }
}