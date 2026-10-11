using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("Choice")]
    public class Choice
    {
        [Key]
        public int ChoiceID { get; set; }

        [Required]
        public int QuestionID { get; set; }

        [Required]
        public string ChoiceText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        [ForeignKey("QuestionID")]
        public virtual Question Question { get; set; }
    }
}