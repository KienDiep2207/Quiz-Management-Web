using DALTWNC_QUIZ.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTWNC_QUIZ.Pages.Admin
{
    public class HomeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HomeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalUsers { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalExamResults { get; set; }
        public double AverageRating { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalCategories { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalCommentsAwaitingApproval { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["ActivePage"] = "Dashboard";

            TotalUsers = await _context.Users.CountAsync();
            TotalQuizzes = await _context.Quizzes.CountAsync();
            TotalExamResults = await _context.QuizAttempts.CountAsync();
            TotalSubjects = await _context.Subjects.CountAsync();
            TotalCategories = await _context.QuestionCategories.CountAsync();
            TotalQuestions = await _context.Questions.CountAsync();
            TotalCommentsAwaitingApproval = await _context.Comments.CountAsync(c => c.IsApproved == false);

            if (await _context.Ratings.AnyAsync())
            {
                AverageRating = await _context.Ratings.AverageAsync(r => r.Stars);
            }
            else
            {
                AverageRating = 0;
            }
        }

        public async Task<IActionResult> OnGetExportAsync(string period)
        {
            DateTime startDate = DateTime.MinValue;
            string timeLabel = "Toàn bộ thời gian";

            switch (period)
            {
                case "day":
                    startDate = DateTime.Today;
                    timeLabel = $"Hôm nay ({DateTime.Now:dd/MM/yyyy})";
                    break;
                case "week":
                    var today = DateTime.Today;
                    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = today.AddDays(-1 * diff).Date;
                    timeLabel = $"Tuần này (Từ {startDate:dd/MM})";
                    break;
                case "month":
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    timeLabel = $"Tháng {DateTime.Now.Month}/{DateTime.Now.Year}";
                    break;
                default:
                    timeLabel = "Toàn bộ thời gian";
                    break;
            }

            var totalUsers = await _context.Users.CountAsync();
            var totalSubjects = await _context.Subjects.CountAsync();
            var totalQuizzesAll = await _context.Quizzes.CountAsync();

            double avgRating = 0;
            if (await _context.Ratings.AnyAsync())
            {
                avgRating = await _context.Ratings.AverageAsync(r => r.Stars);
            }

            var newQuizzesCount = await _context.Quizzes
                .Where(q => q.CreatedDate >= startDate)
                .CountAsync();

            var newAttemptsCount = await _context.QuizAttempts
                .Where(qa => qa.StartTime >= startDate)
                .CountAsync();

            var newCommentsCount = await _context.Comments
                .Where(c => c.CreatedDate >= startDate)
                .CountAsync();

            var newRatingsCount = await _context.Ratings
                .Where(r => r.RatingDate >= startDate)
                .CountAsync();

            var sb = new StringBuilder();
            sb.AppendLine("==================================================");
            sb.AppendLine("         BÁO CÁO THỐNG KÊ HỆ THỐNG QUIZ");
            sb.AppendLine("==================================================");
            sb.AppendLine($"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Phạm vi thống kê:  {timeLabel.ToUpper()}");
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine("");
            sb.AppendLine("[I] HOẠT ĐỘNG TRONG KỲ:");
            sb.AppendLine($" - Số bộ đề thi mới tạo:   {newQuizzesCount}");
            sb.AppendLine($" - Số lượt thí sinh thi:   {newAttemptsCount}");
            sb.AppendLine($" - Số bình luận mới:       {newCommentsCount}");
            sb.AppendLine($" - Số lượt đánh giá mới:   {newRatingsCount}");
            sb.AppendLine("");
            sb.AppendLine("[II] TỔNG QUAN HỆ THỐNG (Lũy kế):");
            sb.AppendLine($" - Tổng số thành viên:     {totalUsers}");
            sb.AppendLine($" - Tổng số môn học:        {totalSubjects}");
            sb.AppendLine($" - Tổng số bộ đề hiện có:  {totalQuizzesAll}");
            sb.AppendLine($" - Đánh giá trung bình:    {avgRating:F1} / 5.0 sao");
            sb.AppendLine("");
            sb.AppendLine("==================================================");
            sb.AppendLine("Người xuất báo cáo: Quản trị viên (Admin)");
            sb.AppendLine("Hệ thống QuizStudy © " + DateTime.Now.Year);

            var content = sb.ToString();
            var bytes = Encoding.UTF8.GetBytes(content);
            var fileName = $"BaoCao_{period}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            return File(bytes, "text/plain", fileName);
        }
    }
}