using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class NewJobCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Title",
                value: "Academia/Research");

            migrationBuilder.InsertData(
                table: "WsJobCategories",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 10, "Consultant" },
                    { 11, "Philanthropy" },
                    { 12, "Central Bank" },
                    { 13, "Diplomat" },
                    { 14, "Regional Energy Agency" },
                    { 15, "Think tank" },
                    { 16, "Responsible investment advisory firm (and actuary)" },
                    { 17, "Multiple affiliations; Gov, NGO, Ac & industry" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.UpdateData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Title",
                value: "Academia/Researcher");
        }
    }
}
