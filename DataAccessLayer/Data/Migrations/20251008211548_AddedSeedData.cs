using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Polls",
                columns: new[] { "Id", "CreatedById", "EndsAt", "IsPublished", "StartsAt", "Summary", "Title", "UpdatedById" },
                values: new object[,]
                {
                    { 1, "Ahmed", new DateOnly(2025, 12, 31), true, new DateOnly(2025, 10, 1), "A survey on the latest technology trends and innovations shaping 2025.", "Technology Trends 2025", null },
                    { 2, "Ahmed", new DateOnly(2026, 1, 15), true, new DateOnly(2025, 9, 15), "Survey on public awareness about sustainability and climate change.", "Environmental Awareness", null }
                });

            migrationBuilder.InsertData(
                table: "Polls",
                columns: new[] { "Id", "CreatedById", "EndsAt", "StartsAt", "Summary", "Title", "UpdatedById" },
                values: new object[] { 3, "Ahmed", new DateOnly(2026, 2, 10), new DateOnly(2025, 11, 10), "Exploring people's health habits, fitness, and dietary preferences.", "Healthcare and Lifestyle Habits", null });

            migrationBuilder.InsertData(
                table: "Polls",
                columns: new[] { "Id", "CreatedById", "EndsAt", "IsPublished", "StartsAt", "Summary", "Title", "UpdatedById" },
                values: new object[] { 4, "Ahmed", new DateOnly(2025, 10, 31), true, new DateOnly(2025, 8, 1), "Opinions on the impact of technology on learning and education systems.", "Education in the Digital Era", null });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Content", "CreatedById", "IsActive", "PollId", "UpdatedById" },
                values: new object[,]
                {
                    { 1, "Which emerging technology will have the biggest impact in 2025?", "Ahmed", true, 1, null },
                    { 2, "Do you believe AI will create more jobs than it replaces?", "Ahmed", true, 1, null },
                    { 3, "How often do you use AI tools like ChatGPT?", "Ahmed", true, 1, null },
                    { 4, "Do you recycle regularly?", "Ahmed", true, 2, null },
                    { 5, "Would you pay more for eco-friendly products?", "Ahmed", true, 2, null },
                    { 6, "Do you think governments should impose carbon taxes?", "Ahmed", true, 2, null },
                    { 7, "How many times a week do you exercise?", "Ahmed", true, 3, null },
                    { 8, "Do you track your daily calorie intake?", "Ahmed", true, 3, null },
                    { 9, "How many hours of sleep do you get on average?", "Ahmed", true, 3, null },
                    { 10, "Do you prefer online or traditional classroom learning?", "Ahmed", true, 4, null },
                    { 11, "Have digital tools improved your learning productivity?", "Ahmed", true, 4, null },
                    { 12, "Should schools fully adopt digital textbooks?", "Ahmed", true, 4, null }
                });

            migrationBuilder.InsertData(
                table: "Answers",
                columns: new[] { "Id", "Content", "QuestionId" },
                values: new object[,]
                {
                    { 1, "Artificial Intelligence", 1 },
                    { 2, "Quantum Computing", 1 },
                    { 3, "Blockchain", 1 },
                    { 4, "Yes, it will create new job categories", 2 },
                    { 5, "No, it will cause job loss", 2 },
                    { 6, "Not sure", 2 },
                    { 7, "Daily", 3 },
                    { 8, "Weekly", 3 },
                    { 9, "Rarely", 3 },
                    { 10, "Always", 4 },
                    { 11, "Sometimes", 4 },
                    { 12, "Never", 4 },
                    { 13, "Yes, definitely", 5 },
                    { 14, "Maybe, if affordable", 5 },
                    { 15, "No, price matters more", 5 },
                    { 16, "Yes, carbon taxes are essential", 6 },
                    { 17, "No, it hurts the economy", 6 },
                    { 18, "Not sure", 6 },
                    { 19, "0–2 times", 7 },
                    { 20, "3–5 times", 7 },
                    { 21, "6+ times", 7 },
                    { 22, "Yes", 8 },
                    { 23, "No", 8 },
                    { 24, "Less than 5 hours", 9 },
                    { 25, "5–7 hours", 9 },
                    { 26, "8 or more hours", 9 },
                    { 27, "Online learning", 10 },
                    { 28, "Traditional classroom", 10 },
                    { 29, "Hybrid (both)", 10 },
                    { 30, "Yes, much improved", 11 },
                    { 31, "Somewhat", 11 },
                    { 32, "No improvement", 11 },
                    { 33, "Yes, digital only", 12 },
                    { 34, "Keep both digital and physical", 12 },
                    { 35, "Prefer only physical", 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 35);

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
                table: "Questions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
