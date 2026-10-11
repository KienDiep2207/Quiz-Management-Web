using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;

namespace DALTWNC_QUIZ.Pages.Customer.QuizDetail
{
    [ValidateAntiForgeryToken]
    public class QuizDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public QuizDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class QuizDetailViewModel
        {
            public int QuizID { get; set; }
            public string QuizTitle { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string SubjectName { get; set; } = string.Empty;
            public int QuestionCount { get; set; }
            public int CalculatedTime { get; set; }
            public string CreatedBy { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public bool IsDynamic { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string returnUrl { get; set; } = "";

        public int SubjectId { get; set; }
        public QuizDetailViewModel? Quiz { get; set; }
        public int CurrentUserRating { get; set; } = 0;
        public bool IsSaved { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            var quizData = await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.QuizID == id && q.IsPublic);

            if (quizData == null)
            {
                return NotFound();
            }

            SubjectId = quizData.SubjectID;

            int questionCount = quizData.TotalQuestions;

            if (questionCount == 0)
            {
                var realCount = await _context.QuizQuestions.CountAsync(qq => qq.QuizID == id);
                if (realCount > 0) questionCount = realCount;
            }

            Quiz = new QuizDetailViewModel
            {
                QuizID = quizData.QuizID,
                QuizTitle = quizData.QuizTitle,
                Description = quizData.Description,
                SubjectName = quizData.Subject?.SubjectName ?? "N/A",
                CreatedBy = quizData.User?.Username ?? "Admin",
                CreatedDate = quizData.CreatedDate,
                QuestionCount = questionCount,
                CalculatedTime = (int)Math.Ceiling(questionCount * 1.5),

                IsDynamic = quizData.IsDynamic
            };

            var username = User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(username))
            {
                var customer = await _context.Customers.AsNoTracking()
                                            .FirstOrDefaultAsync(c => c.Username == username);
                if (customer != null)
                {
                    var userRating = await _context.Ratings.AsNoTracking()
                        .FirstOrDefaultAsync(r => r.QuizID == id && r.CustomerID == customer.CustomerID);
                    if (userRating != null)
                    {
                        CurrentUserRating = userRating.Stars;
                    }
                    IsSaved = await _context.SavedQuizzes
                        .AnyAsync(s => s.QuizID == id && s.CustomerID == customer.CustomerID);
                }
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Page("/Customer/Subjects/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitRatingAsync(int quizId, int stars)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
            {
                return new JsonResult(new { success = false, message = "Bạn cần đăng nhập để đánh giá." }) { StatusCode = 401 };
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);

            if (customer == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy hồ sơ người dùng." }) { StatusCode = 404 };
            }

            var existingRating = await _context.Ratings.FirstOrDefaultAsync(r => r.QuizID == quizId && r.CustomerID == customer.CustomerID);

            if (existingRating != null)
            {
                existingRating.Stars = stars;
                existingRating.RatingDate = DateTime.Now;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    CustomerID = customer.CustomerID,
                    QuizID = quizId,
                    Stars = stars,
                    RatingDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return ViewComponent("RatingSummary", new { quizId = quizId, currentUserRating = stars });
        }

        public async Task<IActionResult> OnPostSubmitCommentAsync(int quizId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new JsonResult(new { success = false, message = "Vui lòng nhập nội dung." }) { StatusCode = 400 };
            }
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
            {
                return new JsonResult(new { success = false, message = "Bạn cần đăng nhập để bình luận." }) { StatusCode = 401 };
            }
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
            if (customer == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy hồ sơ người dùng." }) { StatusCode = 404 };
            }
            _context.Comments.Add(new Comment
            {
                CustomerID = customer.CustomerID,
                QuizID = quizId,
                Content = content,
                CreatedDate = DateTime.Now,
                IsApproved = true
            });
            await _context.SaveChangesAsync();
            return ViewComponent("Comments", new { quizId = quizId });
        }
    }
}