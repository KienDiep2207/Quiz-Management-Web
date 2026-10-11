using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Admin.Quizzes
{
    [Authorize(Roles = "A")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Quiz Quiz { get; set; } = default!;

        [BindProperty]
        public int SubjectFilterId { get; set; }

        public SelectList SubjectSL { get; set; } = default!;
        public int CurrentQuestionCount { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Quiz = await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync(m => m.QuizID == id);

            if (Quiz == null) return NotFound();

            ViewData["ActivePage"] = "Quizzes";
            SubjectSL = new SelectList(await _context.Subjects.AsNoTracking().ToListAsync(), "SubjectID", "SubjectName");

            SubjectFilterId = Quiz.SubjectID;
            CurrentQuestionCount = Quiz.QuizQuestions.Count;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            var quizFromDb = await _context.Quizzes.AsNoTracking()
                                   .FirstOrDefaultAsync(q => q.QuizID == Quiz.QuizID);
            if (quizFromDb == null) return NotFound();

            Quiz.CreatedDate = quizFromDb.CreatedDate;
            Quiz.CreatedBy = quizFromDb.CreatedBy;
            Quiz.SubjectID = SubjectFilterId; 

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            try
            {
                if (Quiz.IsDynamic)
                {
                    var availableCount = await _context.Questions
                                                 .CountAsync(q => q.SubjectID == Quiz.SubjectID);

                    if (Quiz.TotalQuestions > availableCount)
                    {
                        ModelState.AddModelError("Quiz.TotalQuestions", $"Số câu hỏi yêu cầu ({Quiz.TotalQuestions}) vượt quá số câu có sẵn ({availableCount}).");
                        await LoadDropdowns();
                        return Page();
                    }

                    var oldQuestions = _context.QuizQuestions
                                        .Where(qq => qq.QuizID == Quiz.QuizID);
                    if (await oldQuestions.AnyAsync())
                    {
                        _context.QuizQuestions.RemoveRange(oldQuestions);
                        TempData["WarningMessage"] = "Đã chuyển thành bộ đề Tự động. Các câu hỏi cố định cũ đã được gỡ bỏ.";
                    }

                    _context.Quizzes.Update(Quiz);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công bộ đề tự động: {Quiz.QuizTitle}";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var currentQuestionCount = await _context.QuizQuestions
                                                       .CountAsync(qq => qq.QuizID == Quiz.QuizID);

                    if (Quiz.TotalQuestions < currentQuestionCount)
                    {
                        ModelState.AddModelError("Quiz.TotalQuestions", $"Số câu hỏi ({Quiz.TotalQuestions}) không thể ít hơn số câu đã chọn ({currentQuestionCount}).");
                        await LoadDropdowns();
                        CurrentQuestionCount = currentQuestionCount;
                        return Page();
                    }

                    _context.Quizzes.Update(Quiz);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã cập nhật thông tin. Giờ bạn có thể sửa lại câu hỏi.";

                    return RedirectToPage("SelectQuestions",
                        new { quizId = Quiz.QuizID, subjectId = SubjectFilterId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống khi cập nhật: " + ex.Message);
                await LoadDropdowns();
                return Page();
            }
        }

        private async Task LoadDropdowns()
        {
            SubjectSL = new SelectList(await _context.Subjects.AsNoTracking().ToListAsync(), "SubjectID", "SubjectName", SubjectFilterId);
        }
    }
}