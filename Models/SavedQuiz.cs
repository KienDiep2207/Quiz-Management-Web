using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("SavedQuiz")]
    public class SavedQuiz
    {
        [Key]
        public int SavedID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int QuizID { get; set; }

        public DateTime SavedDate { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; }
    }
}