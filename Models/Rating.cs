using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("Rating")]
    public class Rating
    {
        [Key]
        public int RatingID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int QuizID { get; set; }

        public int Stars { get; set; }
        public string? Comment { get; set; }
        public DateTime RatingDate { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; }
    }
}