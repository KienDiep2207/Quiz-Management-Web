using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Customer.Histories
{
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<QuizAttempt> ExamResults { get; set; } = new();

        public string Username { get; set; }

        public string FormatDuration(int totalSeconds)
        {
            if (totalSeconds < 60) { return $"{totalSeconds} giây"; }
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            if (seconds == 0) { return $"{minutes} phút"; }
            return $"{minutes} phút {seconds} giây";
        }

        public async Task<IActionResult> OnGetAsync(string username)
        {
            Username = username;

            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Index");

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
            if (customer == null)
                return NotFound();

            ExamResults = await _context.QuizAttempts
                .Include(e => e.Quiz)
               .Where(e => e.CustomerID == customer.CustomerID &&
                             e.IsCompleted == true)
                .OrderByDescending(e => e.StartTime)
                .ToListAsync();

            return Page();
        }
    }
}
