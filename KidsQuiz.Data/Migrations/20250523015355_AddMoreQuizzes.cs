using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KidsQuiz.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreQuizzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1422));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1425));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1428));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1471));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1474));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Labels", "Title" },
                values: new object[] { new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1344), "A fun quiz to test your basic math skills! Perfect for kids in grades 1-3.", "[\"math\",\"arithmetic\",\"beginner\",\"early-elementary\"]", "Fun Math Quiz for Early Elementary" });

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Labels", "Title" },
                values: new object[] { new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1436), "Test your knowledge about the world around us! Perfect for curious young minds.", "[\"science\",\"nature\",\"facts\",\"early-elementary\"]", "Amazing Science Facts for Kids" });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "Content", "CreatedAt", "Description", "DifficultyLevel", "EstimatedDurationMinutes", "IsGeneratedByLLM", "LLMPrompt", "Labels", "ModifiedAt", "Rating", "RatingCount", "Title" },
                values: new object[,]
                {
                    { 3, "Basic English language skills for young learners", new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1481), "Learn and practice basic English vocabulary and grammar!", 0, 12, false, "", "[\"english\",\"vocabulary\",\"grammar\",\"early-elementary\"]", null, 0.0, 0, "Fun with English Words" },
                    { 4, "Advanced arithmetic and basic algebra for grades 4-6", new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1524), "Test your math skills with these challenging problems!", 2, 20, false, "", "[\"math\",\"arithmetic\",\"algebra\",\"late-elementary\"]", null, 0.0, 0, "Math Challenge for Late Elementary" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AudioUrl", "CorrectAnswerIndex", "CreatedAt", "DifficultyLevel", "Explanation", "ImageUrl", "ModifiedAt", "Options", "Points", "QuizId", "Text" },
                values: new object[,]
                {
                    { 6, "", 2, new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1513), 0, "A noun is a person, place, or thing. 'Dog' is a thing, so it's a noun.", "", null, "[\"Run\",\"Happy\",\"Dog\",\"Quickly\"]", 10, 3, "Which word is a noun?" },
                    { 7, "", 1, new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1516), 0, "The opposite of 'hot' is 'cold'", "", null, "[\"Warm\",\"Cold\",\"Cool\",\"Wet\"]", 10, 3, "What is the opposite of 'hot'?" },
                    { 8, "", 0, new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1519), 1, "The correct sentence is 'I am going to the park' because it uses the correct present continuous tense.", "", null, "[\"I am going to the park\",\"I going to the park\",\"I goes to the park\",\"I go to the park yesterday\"]", 15, 3, "Which sentence is correct?" },
                    { 9, "", 1, new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1555), 1, "12 × 8 = 96", "", null, "[\"86\",\"96\",\"106\",\"116\"]", 15, 4, "What is 12 × 8?" },
                    { 10, "", 2, new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1557), 2, "x + 5 = 12, so x = 12 - 5 = 7", "", null, "[\"5\",\"6\",\"7\",\"8\"]", 20, 4, "If x + 5 = 12, what is x?" },
                    { 11, "", 2, new DateTime(2025, 5, 23, 1, 53, 55, 86, DateTimeKind.Utc).AddTicks(1560), 1, "144 ÷ 12 = 12", "", null, "[\"10\",\"11\",\"12\",\"13\"]", 15, 4, "What is 144 ÷ 12?" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8373));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8376));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8379));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8420));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8423));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Labels", "Title" },
                values: new object[] { new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8287), "A fun quiz to test your basic math skills!", "[\"math\",\"arithmetic\",\"beginner\"]", "Fun Math Quiz for Kids" });

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Labels", "Title" },
                values: new object[] { new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8388), "Test your knowledge about the world around us!", "[\"science\",\"nature\",\"facts\"]", "Amazing Science Facts" });
        }
    }
}
