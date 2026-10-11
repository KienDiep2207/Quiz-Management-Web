using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALTWNC_QUIZ.Migrations
{
    public partial class FinalSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "QuestionCategory",
            //    columns: table => new
            //    {
            //        CategoryID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuestionCategory", x => x.CategoryID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Subject",
            //    columns: table => new
            //    {
            //        SubjectID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SubjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        ParentID = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Subject", x => x.SubjectID);
            //        table.ForeignKey(
            //            name: "FK_Subject_Subject_ParentID",
            //            column: x => x.ParentID,
            //            principalTable: "Subject",
            //            principalColumn: "SubjectID");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "User",
            //    columns: table => new
            //    {
            //        Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        UserRole = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
            //        IsLocked = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_User", x => x.Username);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Question",
            //    columns: table => new
            //    {
            //        QuestionID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryID = table.Column<int>(type: "int", nullable: true),
            //        QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        SubjectID = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Question", x => x.QuestionID);
            //        table.ForeignKey(
            //            name: "FK_Question_QuestionCategory_CategoryID",
            //            column: x => x.CategoryID,
            //            principalTable: "QuestionCategory",
            //            principalColumn: "CategoryID");
            //        table.ForeignKey(
            //            name: "FK_Question_Subject_SubjectID",
            //            column: x => x.SubjectID,
            //            principalTable: "Subject",
            //            principalColumn: "SubjectID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Customer",
            //    columns: table => new
            //    {
            //        CustomerID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CustomerPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
            //        CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CustomerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Customer", x => x.CustomerID);
            //        table.ForeignKey(
            //            name: "FK_Customer_User_Username",
            //            column: x => x.Username,
            //            principalTable: "User",
            //            principalColumn: "Username",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Quiz",
            //    columns: table => new
            //    {
            //        QuizID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SubjectID = table.Column<int>(type: "int", nullable: false),
            //        QuizTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        TotalQuestions = table.Column<int>(type: "int", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsPublic = table.Column<bool>(type: "bit", nullable: false),
            //        IsDynamic = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Quiz", x => x.QuizID);
            //        table.ForeignKey(
            //            name: "FK_Quiz_Subject_SubjectID",
            //            column: x => x.SubjectID,
            //            principalTable: "Subject",
            //            principalColumn: "SubjectID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Quiz_User_CreatedBy",
            //            column: x => x.CreatedBy,
            //            principalTable: "User",
            //            principalColumn: "Username",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Choice",
            //    columns: table => new
            //    {
            //        ChoiceID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuestionID = table.Column<int>(type: "int", nullable: false),
            //        ChoiceText = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IsCorrect = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Choice", x => x.ChoiceID);
            //        table.ForeignKey(
            //            name: "FK_Choice_Question_QuestionID",
            //            column: x => x.QuestionID,
            //            principalTable: "Question",
            //            principalColumn: "QuestionID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Comment",
            //    columns: table => new
            //    {
            //        CommentID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuizID = table.Column<int>(type: "int", nullable: false),
            //        CustomerID = table.Column<int>(type: "int", nullable: false),
            //        Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsApproved = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Comment", x => x.CommentID);
            //        table.ForeignKey(
            //            name: "FK_Comment_Customer_CustomerID",
            //            column: x => x.CustomerID,
            //            principalTable: "Customer",
            //            principalColumn: "CustomerID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Comment_Quiz_QuizID",
            //            column: x => x.QuizID,
            //            principalTable: "Quiz",
            //            principalColumn: "QuizID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizAttempts",
            //    columns: table => new
            //    {
            //        QuizAttemptID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerID = table.Column<int>(type: "int", nullable: false),
            //        QuizID = table.Column<int>(type: "int", nullable: false),
            //        Score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
            //        TotalQuestions = table.Column<int>(type: "int", nullable: false),
            //        CorrectAnswers = table.Column<int>(type: "int", nullable: true),
            //        StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        DurationMinute = table.Column<int>(type: "int", nullable: true),
            //        IsCompleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizAttempts", x => x.QuizAttemptID);
            //        table.ForeignKey(
            //            name: "FK_QuizAttempt_Customer",
            //            column: x => x.CustomerID,
            //            principalTable: "Customer",
            //            principalColumn: "CustomerID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_QuizAttempt_Quiz",
            //            column: x => x.QuizID,
            //            principalTable: "Quiz",
            //            principalColumn: "QuizID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizQuestion",
            //    columns: table => new
            //    {
            //        QuizID = table.Column<int>(type: "int", nullable: false),
            //        QuestionID = table.Column<int>(type: "int", nullable: false),
            //        Score = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizQuestion", x => new { x.QuizID, x.QuestionID });
            //        table.ForeignKey(
            //            name: "FK_QuizQuestion_Question_QuestionID",
            //            column: x => x.QuestionID,
            //            principalTable: "Question",
            //            principalColumn: "QuestionID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_QuizQuestion_Quiz_QuizID",
            //            column: x => x.QuizID,
            //            principalTable: "Quiz",
            //            principalColumn: "QuizID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Rating",
            //    columns: table => new
            //    {
            //        RatingID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerID = table.Column<int>(type: "int", nullable: false),
            //        QuizID = table.Column<int>(type: "int", nullable: false),
            //        Stars = table.Column<int>(type: "int", nullable: false),
            //        Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Rating", x => x.RatingID);
            //        table.ForeignKey(
            //            name: "FK_Rating_Customer_CustomerID",
            //            column: x => x.CustomerID,
            //            principalTable: "Customer",
            //            principalColumn: "CustomerID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Rating_Quiz_QuizID",
            //            column: x => x.QuizID,
            //            principalTable: "Quiz",
            //            principalColumn: "QuizID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SavedQuiz",
            //    columns: table => new
            //    {
            //        SavedID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerID = table.Column<int>(type: "int", nullable: false),
            //        QuizID = table.Column<int>(type: "int", nullable: false),
            //        SavedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SavedQuiz", x => x.SavedID);
            //        table.ForeignKey(
            //            name: "FK_SavedQuiz_Customer_CustomerID",
            //            column: x => x.CustomerID,
            //            principalTable: "Customer",
            //            principalColumn: "CustomerID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_SavedQuiz_Quiz_QuizID",
            //            column: x => x.QuizID,
            //            principalTable: "Quiz",
            //            principalColumn: "QuizID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizAttemptQuestions",
            //    columns: table => new
            //    {
            //        QuizAttemptQuestionID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuizAttemptID = table.Column<int>(type: "int", nullable: false),
            //        QuestionID = table.Column<int>(type: "int", nullable: false),
            //        SelectedChoiceID = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizAttemptQuestions", x => x.QuizAttemptQuestionID);
            //        table.ForeignKey(
            //            name: "FK_AttemptQuestions_Attempts",
            //            column: x => x.QuizAttemptID,
            //            principalTable: "QuizAttempts",
            //            principalColumn: "QuizAttemptID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_AttemptQuestions_Choices",
            //            column: x => x.SelectedChoiceID,
            //            principalTable: "Choice",
            //            principalColumn: "ChoiceID");
            //        table.ForeignKey(
            //            name: "FK_AttemptQuestions_Questions",
            //            column: x => x.QuestionID,
            //            principalTable: "Question",
            //            principalColumn: "QuestionID");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Choice_QuestionID",
            //    table: "Choice",
            //    column: "QuestionID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comment_CustomerID",
            //    table: "Comment",
            //    column: "CustomerID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comment_QuizID",
            //    table: "Comment",
            //    column: "QuizID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Customer_Username",
            //    table: "Customer",
            //    column: "Username",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Question_CategoryID",
            //    table: "Question",
            //    column: "CategoryID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Question_SubjectID",
            //    table: "Question",
            //    column: "SubjectID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Quiz_CreatedBy",
            //    table: "Quiz",
            //    column: "CreatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Quiz_SubjectID",
            //    table: "Quiz",
            //    column: "SubjectID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizAttemptQuestions_QuestionID",
            //    table: "QuizAttemptQuestions",
            //    column: "QuestionID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizAttemptQuestions_QuizAttemptID",
            //    table: "QuizAttemptQuestions",
            //    column: "QuizAttemptID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizAttemptQuestions_SelectedChoiceID",
            //    table: "QuizAttemptQuestions",
            //    column: "SelectedChoiceID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizAttempts_CustomerID",
            //    table: "QuizAttempts",
            //    column: "CustomerID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizAttempts_QuizID",
            //    table: "QuizAttempts",
            //    column: "QuizID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizQuestion_QuestionID",
            //    table: "QuizQuestion",
            //    column: "QuestionID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Rating_CustomerID",
            //    table: "Rating",
            //    column: "CustomerID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Rating_QuizID",
            //    table: "Rating",
            //    column: "QuizID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SavedQuiz_CustomerID",
            //    table: "SavedQuiz",
            //    column: "CustomerID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SavedQuiz_QuizID",
            //    table: "SavedQuiz",
            //    column: "QuizID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Subject_ParentID",
            //    table: "Subject",
            //    column: "ParentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "QuizAttemptQuestions");

            migrationBuilder.DropTable(
                name: "QuizQuestion");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "SavedQuiz");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "Choice");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "QuestionCategory");

            migrationBuilder.DropTable(
                name: "Subject");
        }
    }
}
