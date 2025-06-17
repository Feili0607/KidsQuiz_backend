using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KidsQuiz.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedQuestionBankData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "QuestionBanks",
                columns: new[] { "Id", "AudioUrl", "Category", "CorrectAnswerIndex", "CreatedAt", "DifficultyLevel", "Explanation", "ImageUrl", "IsActive", "ModifiedAt", "Options", "Points", "SuccessRate", "Tags", "TargetAgeGroup", "Text", "UsageCount" },
                values: new object[,]
                {
                    { 1, "", 1, 1, new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7496), 0, "2 + 3 = 5", "", true, null, "[\"4\",\"5\",\"6\",\"7\"]", 10, 0.0, "[\"addition\",\"basic-math\"]", 1, "What is 2 + 3?", 0 },
                    { 2, "", 1, 1, new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7509), 0, "5 - 2 = 3", "", true, null, "[\"2\",\"3\",\"4\",\"5\"]", 10, 0.0, "[\"subtraction\",\"basic-math\"]", 1, "What is 5 - 2?", 0 },
                    { 3, "", 1, 2, new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7519), 1, "4 × 2 = 8", "", true, null, "[\"6\",\"7\",\"8\",\"9\"]", 15, 0.0, "[\"multiplication\",\"basic-math\"]", 1, "What is 4 × 2?", 0 },
                    { 4, "", 0, 0, new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7527), 0, "Plants need both water and sunlight to grow properly", "", true, null, "[\"Water and sunlight\",\"Only water\",\"Only sunlight\",\"Neither water nor sunlight\"]", 10, 0.0, "[\"plants\",\"biology\"]", 1, "What do plants need to grow?", 0 },
                    { 5, "", 0, 1, new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7532), 1, "Mercury is the closest planet to the Sun in our solar system", "", true, null, "[\"Venus\",\"Mercury\",\"Mars\",\"Earth\"]", 15, 0.0, "[\"planets\",\"solar-system\"]", 1, "What is the closest planet to the Sun?", 0 },
                    { 6, "", 9, 2, new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7537), 0, "A noun is a person, place, or thing. 'Dog' is a thing, so it's a noun.", "", true, null, "[\"Run\",\"Happy\",\"Dog\",\"Quickly\"]", 10, 0.0, "[\"grammar\",\"parts-of-speech\"]", 1, "Which word is a noun?", 0 },
                    { 7, "", 9, 1, new DateTime(2025, 6, 17, 11, 21, 3, 697, DateTimeKind.Utc).AddTicks(7540), 0, "The opposite of 'hot' is 'cold'", "", true, null, "[\"Warm\",\"Cold\",\"Cool\",\"Wet\"]", 10, 0.0, "[\"vocabulary\",\"antonyms\"]", 1, "What is the opposite of 'hot'?", 0 }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "QuestionBanks",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(144));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(149));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(152));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(220));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(226));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(292));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(461));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(470));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(535));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(538));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(570));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 551, DateTimeKind.Utc).AddTicks(9971));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(168));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(236));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 11, 19, 36, 552, DateTimeKind.Utc).AddTicks(486));
        }
    }
}
