using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Admin.Quizzes
{
    [Authorize(Roles = "A")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Quiz Quiz { get; set; } = new();

        [BindProperty]
        public int SubjectFilterId { get; set; }

        public SelectList SubjectSL { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Quizzes";
            SubjectSL = new SelectList(await _context.Subjects.AsNoTracking().ToListAsync(), "SubjectID", "SubjectName");

            Quiz.IsPublic = true;
            Quiz.IsDynamic = false;
            Quiz.TotalQuestions = 10;
        }

        public async Task<IActionResult> OnPostCreateQuizAsync()
        {
            ModelState.Remove("Quiz.Subject");
            ModelState.Remove("Quiz.User");
            ModelState.Remove("Quiz.CreatedBy");

            if (SubjectFilterId == 0)
            {
                ModelState.AddModelError("SubjectFilterId", "Vui lòng chọn môn học.");
            }
            if (Quiz.TotalQuestions <= 0)
            {
                ModelState.AddModelError("Quiz.TotalQuestions", "Số lượng câu hỏi phải lớn hơn 0.");
            }
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            Quiz.CreatedBy = User.Identity?.Name ?? "Admin";
            Quiz.CreatedDate = DateTime.Now;
            Quiz.SubjectID = SubjectFilterId;

            try
            {
                if (Quiz.IsDynamic)
                {
                    var availableCount = await _context.Questions
                                                 .CountAsync(q => q.SubjectID == Quiz.SubjectID);

                    if (Quiz.TotalQuestions > availableCount)
                    {
                        ModelState.AddModelError("Quiz.TotalQuestions", $"Số câu hỏi yêu cầu ({Quiz.TotalQuestions}) vượt quá số câu có sẵn ({availableCount}) trong môn này.");
                        await OnGetAsync();
                        return Page();
                    }

                    _context.Quizzes.Add(Quiz);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã tạo thành công bộ đề tự động: {Quiz.QuizTitle}";
                    return RedirectToPage("./Index");
                }
                else
                {
                    _context.Quizzes.Add(Quiz);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Bộ đề đã được tạo. Giờ hãy thêm câu hỏi!";

                    return RedirectToPage("SelectQuestions",
                        new { quizId = Quiz.QuizID, subjectId = SubjectFilterId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống khi tạo đề: " + ex.Message);
                await OnGetAsync();
                return Page();
            }
        }
    }
}