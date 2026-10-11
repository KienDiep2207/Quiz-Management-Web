using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Customer.Profiles
{
    public class EditProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DALTWNC_QUIZ.Models.Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Index");

            Customer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);

            if (Customer == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string username)
        {
            if (!ModelState.IsValid)
                return Page();

            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Index");

            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);

            if (customerInDb == null)
                return NotFound();

            customerInDb.CustomerName = Customer.CustomerName;
            customerInDb.CustomerEmail = Customer.CustomerEmail;
            customerInDb.CustomerPhone = Customer.CustomerPhone;
            customerInDb.CustomerAddress = Customer.CustomerAddress;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "C?p nh?t th�ng tin th�nh c�ng!";
            return RedirectToPage("/Customer/Profiles/Profile", new { username = username });
        }
    }
}
