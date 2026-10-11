using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALTWNC_QUIZ.Migrations
{
    public partial class AllowNullSelectedChoiceID : Migration
    {
        // 🔻 HÀM UP: SẼ ĐƯỢC CHẠY KHI Update-Database 🔻
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Code này ra lệnh cho CSDL:
            // "Tìm bảng QuizAttemptQuestions, tìm cột SelectedChoiceID,
            // và thay đổi nó để cho phép giá trị NULL"
            migrationBuilder.AlterColumn<int>(
                name: "SelectedChoiceID",
                table: "QuizAttemptQuestions",
                type: "int",
                nullable: true, // <-- Cho phép NULL
                oldClrType: typeof(int),
                oldType: "int");
        }

        // 🔻 HÀM DOWN: (Để phòng khi cần gỡ bỏ) 🔻
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Code này làm ngược lại
            migrationBuilder.AlterColumn<int>(
                name: "SelectedChoiceID",
                table: "QuizAttemptQuestions",
                type: "int",
                nullable: false, // <-- Không cho phép NULL
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}