using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.Comments
{
    [Authorize(Roles = "A")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Comment> CommentList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string query { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Comments";
            ViewData["CurrentQuery"] = query;

            var commentQuery = _context.Comments
                .Include(c => c.Customer)
                .Include(c => c.Quiz)
                .OrderByDescending(c => c.CreatedDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query))
            {
                commentQuery = commentQuery.Where(c =>
                    (c.Customer != null && c.Customer.CustomerName.Contains(query)) ||
                    (c.Quiz != null && c.Quiz.QuizTitle.Contains(query)) ||
                    c.Content.Contains(query)
                );
            }

            CommentList = await commentQuery.ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsApproved = true;
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Duyệt bình luận thành công!";
                }
                catch { TempData["ErrorMessage"] = "Lỗi khi duyệt bình luận."; }
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsApproved = false;
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ẩn bình luận thành công!";
                }
                catch { TempData["ErrorMessage"] = "Lỗi khi ẩn bình luận."; }
            }
            return RedirectToPage("./Index");
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            try
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa bình luận thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa.";
            }

            return RedirectToPage("./Index");
        }
    }
}