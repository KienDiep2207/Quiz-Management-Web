using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("QuizAttemptQuestions")]
    public class QuizAttemptQuestion
    {
        [Key]
        [Column("QuizAttemptQuestionID")]
        public int QuizAttemptQuestionID { get; set; }

        [Required]
        [Column("QuizAttemptID")]
        public int QuizAttemptID { get; set; }

        public virtual QuizAttempt QuizAttempt { get; set; }

        [Required]
        public int QuestionID { get; set; }

        public virtual Question Question { get; set; }

        public int? SelectedChoiceID { get; set; }
        public virtual Choice SelectedChoice { get; set; }
    }
}