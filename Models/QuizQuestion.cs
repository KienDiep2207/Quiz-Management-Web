using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("QuizQuestion")]
    public class QuizQuestion
    {
        public int QuizID { get; set; }
        public int QuestionID { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Score { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}