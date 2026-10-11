using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DALTWNC_QUIZ.Models
{
    [Table("Quiz")]
    public class Quiz
    {
        [Key]
        public int QuizID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Required]
        [StringLength(255)]
        public string QuizTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; } 

        [Required]
        public int TotalQuestions { get; set; } = 10;

        public DateTime CreatedDate { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDynamic { get; set; }

        [ForeignKey("SubjectID")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new HashSet<QuizQuestion>();
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new HashSet<QuizAttempt>();
        public virtual ICollection<SavedQuiz> SavedQuizzes { get; set; } = new HashSet<SavedQuiz>();
        public virtual ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}