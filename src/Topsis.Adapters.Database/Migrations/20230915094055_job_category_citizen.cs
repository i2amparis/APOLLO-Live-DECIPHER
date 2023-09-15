using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class job_category_citizen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WsJobCategories",
                columns: new[] { "Id", "Title" },
                values: new object[] { 18, "Citizen" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WsJobCategories",
                keyColumn: "Id",
                keyValue: 18);
        }
    }
}
