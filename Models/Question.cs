using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DALTWNC_QUIZ.Models
{
    [Table("Question")]
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }

        public int? CategoryID { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public int SubjectID { get; set; }

        [ForeignKey("SubjectID")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("CategoryID")]
        public virtual QuestionCategory? QuestionCategory { get; set; }

        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new HashSet<QuizQuestion>();
        public virtual ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();
    }
}