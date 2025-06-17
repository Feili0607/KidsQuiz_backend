using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KidsQuiz.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "Content", "CreatedAt", "Description", "DifficultyLevel", "EstimatedDurationMinutes", "IsGeneratedByLLM", "LLMPrompt", "Labels", "ModifiedAt", "Rating", "RatingCount", "Title" },
                values: new object[,]
                {
                    { 1, "Basic arithmetic operations for young learners", new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8287), "A fun quiz to test your basic math skills!", 0, 10, false, "", "[\"math\",\"arithmetic\",\"beginner\"]", null, 0.0, 0, "Fun Math Quiz for Kids" },
                    { 2, "Basic science concepts for curious minds", new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8388), "Test your knowledge about the world around us!", 1, 15, false, "", "[\"science\",\"nature\",\"facts\"]", null, 0.0, 0, "Amazing Science Facts" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AudioUrl", "CorrectAnswerIndex", "CreatedAt", "DifficultyLevel", "Explanation", "ImageUrl", "ModifiedAt", "Options", "Points", "QuizId", "Text" },
                values: new object[,]
                {
                    { 1, "", 1, new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8373), 0, "2 + 3 = 5", "", null, "[\"4\",\"5\",\"6\",\"7\"]", 10, 1, "What is 2 + 3?" },
                    { 2, "", 1, new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8376), 0, "5 - 2 = 3", "", null, "[\"2\",\"3\",\"4\",\"5\"]", 10, 1, "What is 5 - 2?" },
                    { 3, "", 2, new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8379), 1, "4 × 2 = 8", "", null, "[\"6\",\"7\",\"8\",\"9\"]", 15, 1, "What is 4 × 2?" },
                    { 4, "", 0, new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8420), 0, "Plants need both water and sunlight to grow properly", "", null, "[\"Water and sunlight\",\"Only water\",\"Only sunlight\",\"Neither water nor sunlight\"]", 10, 2, "What do plants need to grow?" },
                    { 5, "", 1, new DateTime(2025, 5, 23, 1, 23, 36, 647, DateTimeKind.Utc).AddTicks(8423), 1, "Mercury is the closest planet to the Sun in our solar system", "", null, "[\"Venus\",\"Mercury\",\"Mars\",\"Earth\"]", 15, 2, "What is the closest planet to the Sun?" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
