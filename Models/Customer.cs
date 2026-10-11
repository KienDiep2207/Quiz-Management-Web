using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DALTWNC_QUIZ.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; } = default!;

        [ForeignKey("Username")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new HashSet<QuizAttempt>();
        public virtual ICollection<SavedQuiz> SavedQuizzes { get; set; } = new HashSet<SavedQuiz>();
        public virtual ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}