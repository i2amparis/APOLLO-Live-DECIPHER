using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class UserGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "GenderId",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "AspNetUsers");
        }
    }
}
