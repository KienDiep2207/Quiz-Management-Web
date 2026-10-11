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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Question Question { get; set; } = default!;

        [BindProperty]
        public List<ChoiceInputModel> Choices { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn một đáp án đúng cho câu hỏi.")]
        public int? CorrectChoiceIndex { get; set; }

        public SelectList SubjectSL { get; set; }
        public SelectList CategorySL { get; set; }

        public class ChoiceInputModel
        {
            public int ChoiceID { get; set; }
            [Required(ErrorMessage = "Vui lòng nhập nội dung đáp án.")]
            public string ChoiceText { get; set; } = string.Empty;
            public bool IsCorrect { get; set; }
        }

        private async Task LoadDependenciesAsync(int? subjectId = null, int? categoryId = null)
        {
            SubjectSL = new SelectList(await _context.Subjects.AsNoTracking().ToListAsync(), "SubjectID", "SubjectName", subjectId);
            CategorySL = new SelectList(await _context.QuestionCategories.AsNoTracking().ToListAsync(), "CategoryID", "CategoryName", categoryId);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ViewData["ActivePage"] = "QuestionsBank";

            Question = await _context.Questions
                .Include(q => q.Choices)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.QuestionID == id);

            if (Question == null) return NotFound();

            await LoadDependenciesAsync(Question.SubjectID, Question.CategoryID);

            Choices = Question.Choices.Select(c => new ChoiceInputModel
            {
                ChoiceID = c.ChoiceID,
                ChoiceText = c.ChoiceText,
                IsCorrect = c.IsCorrect
            }).ToList();

            CorrectChoiceIndex = Choices.FindIndex(c => c.IsCorrect);

            while (Choices.Count < 4)
            {
                Choices.Add(new ChoiceInputModel());
            }

            if (CorrectChoiceIndex == -1) CorrectChoiceIndex = 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDependenciesAsync(Question.SubjectID, Question.CategoryID);

            ModelState.Remove("Question.Subject");
            ModelState.Remove("Question.QuestionCategory");
            ModelState.Remove("Choices.ChoiceID");

            var validChoicesInput = Choices.Where(c => !string.IsNullOrWhiteSpace(c.ChoiceText)).ToList();
            if (validChoicesInput.Count < 2)
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

            validChoicesInput = Choices.Where(c => !string.IsNullOrWhiteSpace(c.ChoiceText)).ToList();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Attach(Question).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var existingChoices = await _context.Choices.Where(c => c.QuestionID == Question.QuestionID).ToListAsync();

                    var currentValidIds = validChoicesInput.Where(v => v.ChoiceID > 0).Select(v => v.ChoiceID).ToList();
                    var choicesToDelete = existingChoices.Where(ec => !currentValidIds.Contains(ec.ChoiceID)).ToList();
                    _context.Choices.RemoveRange(choicesToDelete);

                    foreach (var choiceInput in validChoicesInput)
                    {
                        if (choiceInput.ChoiceID > 0)
                        {
                            var choiceToUpdate = existingChoices.FirstOrDefault(ec => ec.ChoiceID == choiceInput.ChoiceID);
                            if (choiceToUpdate != null)
                            {
                                choiceToUpdate.ChoiceText = choiceInput.ChoiceText;
                                choiceToUpdate.IsCorrect = choiceInput.IsCorrect;
                            }
                        }
                        else
                        {
                            _context.Choices.Add(new Choice { QuestionID = Question.QuestionID, ChoiceText = choiceInput.ChoiceText, IsCorrect = choiceInput.IsCorrect });
                        }
                    }
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Cập nhật câu hỏi thành công!";
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "Lỗi đồng thời cơ sở dữ liệu. Vui lòng thử lại.";
                    return RedirectToPage("./Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Lỗi hệ thống: " + ex.Message);
                    return Page();
                }
            }
        }
    }
}