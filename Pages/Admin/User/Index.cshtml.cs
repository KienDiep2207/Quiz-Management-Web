using DALTWNC_QUIZ.Data;
using System.Linq;
using DALTWNC_QUIZ.Models;
using DALTWNC_QUIZ.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.User
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<UserViewModel> UserList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Users";

            ViewData["CurrentQuery"] = query;

            var userQuery = from u in _context.Users
                            join c in _context.Customers on u.Username equals c.Username into userCustomers
                            from c in userCustomers.DefaultIfEmpty()
                            select new UserViewModel
                            {
                                User = u,
                                Customer = c
                            };

            if (!string.IsNullOrEmpty(query))
            {
                userQuery = userQuery.Where(uvm =>
                    uvm.User.Username.Contains(query) ||
                    (uvm.Customer != null && uvm.Customer.CustomerName.Contains(query))
                );
            }
            if (!await userQuery.AnyAsync())
            {
                TempData["NoResults"] = true;
            }

        UserList = await userQuery
                             .AsNoTracking()
                             .ToListAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == id);

            if (user == null) return NotFound();

            if (user.UserRole == "A")
            {
                var hasQuizzes = await _context.Quizzes.AnyAsync(q => q.CreatedBy == id);
                if (hasQuizzes)
                {
                    TempData["ErrorMessage"] = "Lỗi! Không thể xóa Admin này vì họ đã tạo Bộ đề.";
                    return RedirectToPage("./Index");
                }
            }

            if (customer != null)
            {
                var hasResults = await _context.QuizAttempts.AnyAsync(r => r.CustomerID == customer.CustomerID);
                var hasComments = await _context.Comments.AnyAsync(c => c.CustomerID == customer.CustomerID);
                var hasRatings = await _context.Ratings.AnyAsync(r => r.CustomerID == customer.CustomerID);

                if (hasResults || hasComments || hasRatings)
                {
                    TempData["ErrorMessage"] = "Lỗi! Không thể xóa User này vì họ đã có lịch sử (làm bài, bình luận, v.v.).";
                    return RedirectToPage("./Index");
                }
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (customer != null)
                    {
                        var savedQuizzes = _context.SavedQuizzes.Where(s => s.CustomerID == customer.CustomerID);
                        _context.SavedQuizzes.RemoveRange(savedQuizzes);
                        await _context.SaveChangesAsync();

                        _context.Customers.Remove(customer);
                        await _context.SaveChangesAsync();
                    }

                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Xóa người dùng thành công!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Lỗi nghiêm trọng khi xóa: " + ex.Message;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
