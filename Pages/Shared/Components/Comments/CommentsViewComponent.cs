using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace DALTWNC_QUIZ.Pages.Shared.Components.Comments
{
    public class CommentsComponentViewModel
    {
        public int QuizId { get; set; }
        public List<CommentViewModel> ExistingComments { get; set; } = new List<CommentViewModel>();
    }

    public class CommentViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class CommentsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CommentsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int quizId)
        {
            var comments = await _context.Comments
                .Where(c => c.QuizID == quizId && c.IsApproved)
                .Include(c => c.Customer)
                .OrderByDescending(c => c.CreatedDate)
                .Select(c => new CommentViewModel
                {
                    UserName = c.Customer.Username,
                    CreatedDate = c.CreatedDate,
                    Content = c.Content
                })
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new CommentsComponentViewModel
            {
                QuizId = quizId,
                ExistingComments = comments
            };

            return View(viewModel);
        }
    }
}