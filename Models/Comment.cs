using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public int QuizID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; }
    }
}