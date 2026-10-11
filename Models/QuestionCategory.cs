using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("QuestionCategory")]
    public class QuestionCategory
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(255)]
        public string CategoryName { get; set; } = string.Empty;

        public virtual ICollection<Question> Questions { get; set; } = new HashSet<Question>();
    }
}