using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Admin.Quizzes
{
    [Authorize(Roles = "A")]
    public class SelectQuestionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SelectQuestionsModel(ApplicationDbContext context) => _context = context;

        public Quiz Quiz { get; set; } = default!;
        public DALTWNC_QUIZ.Models.Subject Subject { get; set; } = default!;
        public IList<Question> AvailableQuestions { get; set; } = new List<Question>();
        public HashSet<int> CurrentlySelectedIds { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int QuizId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SubjectId { get; set; }

        [BindProperty]
        public Dictionary<string, bool> QuestionSelections { get; set; } = new();

        [BindProperty]
        public List<int> SelectedQuestionIds { get; set; } = new();

        private async Task UpdateQuestionScores(int quizId, int totalQuestionsInQuiz)
        {
            if (totalQuestionsInQuiz == 0) return;

            decimal totalScore = 10.0m;
            decimal scorePerQuestion = Math.Round(totalScore / totalQuestionsInQuiz, 2);

            var quizQuestionsToUpdate = await _context.QuizQuestions
        .Where(qq => qq.QuizID == quizId)
        .ToListAsync();

            foreach (var qq in quizQuestionsToUpdate)
            {
                if (qq.Score != scorePerQuestion)
                {
                    qq.Score = scorePerQuestion;
                    _context.Entry(qq).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> OnGetAsync(int quizId, int subjectId)
        {
            ViewData["ActivePage"] = "Quizzes";
            QuizId = quizId;
            SubjectId = subjectId;

            Quiz = await _context.Quizzes.FindAsync(quizId);
            Subject = await _context.Subjects.FindAsync(subjectId);

            if (Quiz == null || Subject == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy Bộ đề hoặc Môn học được chỉ định.";
                return RedirectToPage("Index");
            }

            AvailableQuestions = await _context.Questions
                .Where(q => q.SubjectID == subjectId)
                .Include(q => q.QuestionCategory)
                .AsNoTracking()
                .ToListAsync();

            CurrentlySelectedIds = (await _context.QuizQuestions
                .Where(qq => qq.QuizID == quizId)
                .Select(qq => qq.QuestionID)
                .ToListAsync())
                .ToHashSet();

            return Page();
        }

        public async Task<IActionResult> OnPostFinalizeAsync(string handler, int quizId, int subjectId)
        {
            Quiz = await _context.Quizzes.FindAsync(quizId) ?? throw new Exception("Quiz not found");

            List<int> selectedQuestionIds;

            if (handler == "Randomize")
            {
                var allQuestionIds = await _context.Questions
                    .Where(q => q.SubjectID == subjectId && q.SubjectID != null)
                    .Select(q => q.QuestionID).ToListAsync();

                if (allQuestionIds == null || allQuestionIds.Count < Quiz.TotalQuestions)
                {
                    TempData["ErrorMessage"] = $"Không đủ câu hỏi ({allQuestionIds?.Count ?? 0}) để random {Quiz.TotalQuestions} câu.";
                    return RedirectToPage(new { quizId, subjectId });
                }

                Random rand = new();
                selectedQuestionIds = allQuestionIds.OrderBy(_ => rand.Next())
                    .Take(Quiz.TotalQuestions).ToList();
            }
            else
            {
                selectedQuestionIds = SelectedQuestionIds?.Where(id => id != 0).ToList() ?? new List<int>();

                if (selectedQuestionIds.Count > Quiz.TotalQuestions)
                {
                    TempData["ErrorMessage"] = $"Bạn đã chọn {selectedQuestionIds.Count} câu, vượt quá số lượng mong muốn ({Quiz.TotalQuestions}).";
                    return RedirectToPage(new { quizId, subjectId });
                }
            }

            if (!selectedQuestionIds.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn hoặc Random ít nhất một câu hỏi.";
                return RedirectToPage(new { quizId, subjectId });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.QuizQuestions
                    .Where(qq => qq.QuizID == quizId)
                    .ToListAsync();
                if (existing.Any()) _context.QuizQuestions.RemoveRange(existing);

                var newLinks = selectedQuestionIds
                .Select(qId => new QuizQuestion { QuizID = quizId, QuestionID = qId })
                .ToList();

                _context.QuizQuestions.AddRange(newLinks);
                await _context.SaveChangesAsync();

                await UpdateQuestionScores(quizId, selectedQuestionIds.Count);

                await transaction.CommitAsync();
                TempData["SuccessMessage"] = $"Đã thêm {selectedQuestionIds.Count} câu hỏi vào bộ đề {Quiz.QuizTitle}.";
                return RedirectToPage("Index");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Lỗi: Câu hỏi trùng lặp trong bộ đề.";
                return RedirectToPage(new { quizId, subjectId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Lỗi khi lưu: " + ex.Message;
                return RedirectToPage(new { quizId, subjectId });
            }
        }
    }
}
