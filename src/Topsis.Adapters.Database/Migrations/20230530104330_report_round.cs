using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class report_round : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Round",
                table: "WsWorkspacesReports",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Round",
                table: "WsWorkspacesReports");
        }
    }
}
