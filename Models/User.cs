using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [StringLength(255)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        public string UserRole { get; set; } = string.Empty;

        [Required]
        public bool IsLocked { get; set; } = false;

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();
    }
}
