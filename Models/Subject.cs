using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DALTWNC_QUIZ.Models
{
    [Table("Subject")]
    public class Subject
    {
        [Key]
        public int SubjectID { get; set; }

        [Required]
        [StringLength(255)]
        public string SubjectName { get; set; } = string.Empty;

        public int? ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual Subject? ParentSubject { get; set; }

        public virtual ICollection<Subject> ChildSubjects { get; set; } = new HashSet<Subject>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();
    }
}