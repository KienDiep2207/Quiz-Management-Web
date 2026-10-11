using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DALTWNC_QUIZ.Pages.Admin.QuestionsBank
{
    [Authorize(Roles = "A")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Question Question { get; set; } = new();

        [BindProperty]
        public List<ChoiceInputModel> Choices { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn một đáp án đúng cho câu hỏi.")]
        public int? CorrectChoiceIndex { get; set; }

        public SelectList SubjectSL { get; set; }
        public SelectList CategorySL { get; set; }

        public class ChoiceInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập nội dung đáp án.")]
            public string ChoiceText { get; set; } = string.Empty;
            public bool IsCorrect { get; set; }
        }

        private async Task LoadDependenciesAsync()
        {
            SubjectSL = new SelectList(await _context.Subjects.AsNoTracking().ToListAsync(), "SubjectID", "SubjectName");
            CategorySL = new SelectList(await _context.QuestionCategories.AsNoTracking().ToListAsync(), "CategoryID", "CategoryName");
        }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "QuestionsBank";
            await LoadDependenciesAsync();
            Choices = new List<ChoiceInputModel> { new(), new(), new(), new() };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDependenciesAsync();
            ModelState.Remove("Question.Subject");
            ModelState.Remove("Question.QuestionCategory");


            var validChoices = Choices.Where(c => !string.IsNullOrWhiteSpace(c.ChoiceText)).ToList();
            if (validChoices.Count < 2)
                ModelState.AddModelError(string.Empty, "Phải có ít nhất hai đáp án có nội dung.");

            if (!ModelState.IsValid)
            {
                if (CorrectChoiceIndex.HasValue) CorrectChoiceIndex = CorrectChoiceIndex.Value;
                return Page();
            }

            Choices.ForEach(c => c.IsCorrect = false);

            if (CorrectChoiceIndex.HasValue && CorrectChoiceIndex.Value >= 0 && CorrectChoiceIndex.Value < Choices.Count)
            {
                Choices[CorrectChoiceIndex.Value].IsCorrect = true;
            }

            validChoices = Choices.Where(c => !string.IsNullOrWhiteSpace(c.ChoiceText)).ToList();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Questions.Add(Question);
                    await _context.SaveChangesAsync();

                    foreach (var choiceInput in validChoices)
                    {
                        _context.Choices.Add(new Choice
                        {
                            QuestionID = Question.QuestionID,
                            ChoiceText = choiceInput.ChoiceText,
                            IsCorrect = choiceInput.IsCorrect
                        });
                    }
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Tạo câu hỏi mới thành công!";
                    return RedirectToPage("./Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu: " + ex.Message);
                    return Page();
                }
            }
        }
    }
}