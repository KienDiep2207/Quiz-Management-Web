using DALTWNC_QUIZ.Data;
using DALTWNC_QUIZ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Pages.Customer.Subjects
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class SubjectViewModel
        {
            public int SubjectID { get; set; }
            public string SubjectName { get; set; }
            public int QuizCount { get; set; }
        }

        public IList<SubjectViewModel> SubjectList { get; set; } = new List<SubjectViewModel>();

        public async Task OnGetAsync()
        {
            SubjectList = await _context.Subjects
                .OrderBy(s => s.SubjectName)
                .AsNoTracking()
                .Select(s => new SubjectViewModel
                {
                    SubjectID = s.SubjectID,
                    SubjectName = s.SubjectName,
                    QuizCount = s.Quizzes.Count(q => q.IsPublic)
                })
                .ToListAsync();
        }
    }
}
