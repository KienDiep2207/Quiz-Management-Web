//using DALTWNC_QUIZ.Models;
//using Microsoft.EntityFrameworkCore;

//namespace DALTWNC_QUIZ.Data
//{
//    public class ApplicationDbContext : DbContext
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        public DbSet<User> Users { get; set; } = default!;
//        public DbSet<Customer> Customers { get; set; } = default!;

//        // Các bảng khác (Subject, Quiz, ...) bạn sẽ thêm dần sau.
//        public DbSet<Subject> Subjects { get; set; } = default!;
//        public DbSet<Quiz> Quizzes { get; set; } = default!;
//        public DbSet<QuestionCategory> QuestionCategories { get; set; } = default!;
//        public DbSet<Question> Questions { get; set; } = default!;
//        public DbSet<Choice> Choices { get; set; } = default!;
//        public DbSet<QuizAttempt> QuizAttempts { get; set; } = default!;
//        public DbSet<SavedQuiz> SavedQuizzes { get; set; } = default!;
//        public DbSet<Rating> Ratings { get; set; } = default!;
//        public DbSet<Comment> Comments { get; set; } = default!;
//        public DbSet<QuizQuestion> QuizQuestions { get; set; } = default!;
//        public DbSet<QuizAttemptQuestion> QuizAttemptQuestions { get; set; }

//        // ================================================================
//        // ✅ THÊM ĐOẠN CODE NÀY VÀO
//        // ================================================================
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);
//            modelBuilder.Entity<QuizQuestion>()
//                .HasKey(qq => new { qq.QuizID, qq.QuestionID });
//        }
//    }
//}
using DALTWNC_QUIZ.Models;
using Microsoft.EntityFrameworkCore;

namespace DALTWNC_QUIZ.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<Subject> Subjects { get; set; } = default!;
        public DbSet<Quiz> Quizzes { get; set; } = default!;
        public DbSet<QuestionCategory> QuestionCategories { get; set; } = default!;
        public DbSet<Question> Questions { get; set; } = default!;
        public DbSet<Choice> Choices { get; set; } = default!;
        public DbSet<SavedQuiz> SavedQuizzes { get; set; } = default!;
        public DbSet<Rating> Ratings { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<QuizQuestion> QuizQuestions { get; set; } = default!;

        // === CÁC BẢNG ĐÃ ĐỔI TÊN ===
        public DbSet<QuizAttempt> QuizAttempts { get; set; } = default!;
        public DbSet<QuizAttemptQuestion> QuizAttemptQuestions { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bảng QuizQuestion (cũ, vẫn dùng)
            modelBuilder.Entity<QuizQuestion>()
                .HasKey(qq => new { qq.QuizID, qq.QuestionID });

            // === THÊM PHẦN NÀY ĐỂ MAP CÁC BẢNG ĐÃ ĐỔI TÊN ===

            // Map bảng QuizAttempt (tên cũ ExamResult)
            modelBuilder.Entity<QuizAttempt>(entity =>
            {
                // Chỉ định tên bảng mới
                entity.ToTable("QuizAttempts");

                // Chỉ định PK mới
                entity.HasKey(e => e.QuizAttemptID);

                // Map các cột đã đổi tên
                entity.Property(e => e.StartTime).HasColumnName("StartTime");

                // Chỉ định khóa ngoại mới
                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.QuizAttempts) // <--- SỬA LẠI THÀNH THẾ NÀY
                      .HasForeignKey(d => d.CustomerID)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_QuizAttempt_Customer");

                entity.HasOne(d => d.Quiz)
                      .WithMany(p => p.QuizAttempts) // <--- SỬA LẠI THÀNH THẾ NÀY
                      .HasForeignKey(d => d.QuizID)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_QuizAttempt_Quiz");
            });

            // Map bảng QuizAttemptQuestion (tên cũ UserAnswers)
            modelBuilder.Entity<QuizAttemptQuestion>(entity =>
            {
                // Chỉ định tên bảng mới
                entity.ToTable("QuizAttemptQuestions");

                // Chỉ định PK mới
                entity.HasKey(e => e.QuizAttemptQuestionID);

                // Chỉ định các khóa ngoại mới
                entity.HasOne(d => d.QuizAttempt)
                    .WithMany(p => p.QuizAttemptQuestions) // Liên kết 1-nhiều
                    .HasForeignKey(d => d.QuizAttemptID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AttemptQuestions_Attempts"); // Tên mới

                entity.HasOne(d => d.Question)
                    .WithMany()
                    .HasForeignKey(d => d.QuestionID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AttemptQuestions_Questions"); // Tên mới

                entity.HasOne(d => d.SelectedChoice)
                    .WithMany()
                    .HasForeignKey(d => d.SelectedChoiceID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AttemptQuestions_Choices"); // Tên mới
            });
        }
    }
}