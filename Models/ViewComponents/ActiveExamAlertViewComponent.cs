using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Models.ViewComponents
{
    public class ActiveExamAlertViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ActiveExamAlertViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Content("");

            var customer = await _context.Customers.AsNoTracking()
                                .FirstOrDefaultAsync(c => c.Username == username);

            if (customer == null) return Content("");

            var activeAttempt = await _context.QuizAttempts
                .Include(qa => qa.Quiz)
                .OrderByDescending(qa => qa.StartTime)
                .FirstOrDefaultAsync(qa => qa.CustomerID == customer.CustomerID && qa.IsCompleted == false);

            if (activeAttempt == null) return Content("");

            var currentPath = HttpContext.Request.Path.Value?.ToLower();

            var examUrl = $"/customer/exam/take/{activeAttempt.QuizID}";
            if (currentPath != null && currentPath.Contains(examUrl))
            {
                return Content("");
            }

            return View(activeAttempt);
        }
    }
}