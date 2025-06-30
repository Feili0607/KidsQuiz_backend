using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidsQuiz.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKidIdToQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KidId",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4509));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4528));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4534));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4542));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4546));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4550));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4554));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4277));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4281));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4289));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4342));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4349));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4402));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4434));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4439));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4484));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4487));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4490));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "KidId" },
                values: new object[] { new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4151), null });

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "KidId" },
                values: new object[] { new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4300), null });

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "KidId" },
                values: new object[] { new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4360), null });

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "KidId" },
                values: new object[] { new DateTime(2025, 6, 30, 7, 1, 17, 182, DateTimeKind.Utc).AddTicks(4446), null });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_KidId",
                table: "Quizzes",
                column: "KidId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Kids_KidId",
                table: "Quizzes",
                column: "KidId",
                principalTable: "Kids",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Kids_KidId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_KidId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "KidId",
                table: "Quizzes");

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7496));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7509));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7519));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7527));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7532));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7537));

            migrationBuilder.UpdateData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7540));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7311));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7317));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7320));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7377));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7380));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7427));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7431));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7434));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7475));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7478));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7481));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7210));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7338));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7388));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7440));
        }
    }
}
