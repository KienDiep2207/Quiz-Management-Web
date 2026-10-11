using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALTWNC_QUIZ.Models
{
    [Table("QuizAttempts")]
    public class QuizAttempt
    {
        public QuizAttempt()
        {
            QuizAttemptQuestions = new HashSet<QuizAttemptQuestion>();
        }

        [Key]
        [Column("QuizAttemptID")]
        public int QuizAttemptID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int QuizID { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Score { get; set; }

        public int TotalQuestions { get; set; }

        public int? CorrectAnswers { get; set; }
        
        [Column("StartTime")]
        public DateTime StartTime { get; set; }
        
        public int? DurationMinute { get; set; }

        public bool IsCompleted { get; set; }


        public virtual Customer Customer { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<QuizAttemptQuestion> QuizAttemptQuestions { get; set; }
    }
}