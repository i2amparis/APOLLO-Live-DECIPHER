using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class WorkspaceImportKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportKey",
                table: "WsWorkspaces",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportKey",
                table: "WsWorkspaces");

        }
    }
}
