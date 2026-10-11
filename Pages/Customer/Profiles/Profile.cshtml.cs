using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace DALTWNC_QUIZ.Pages.Customer.Profiles
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public DALTWNC_QUIZ.Models.Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Index");

            Customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Username == username);

            if (Customer == null)
                return NotFound();

            return Page();
        }
    }
}