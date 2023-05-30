using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class report_round_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WsWorkspacesReports_WorkspaceId_Algorithm",
                table: "WsWorkspacesReports");

            migrationBuilder.AlterColumn<short>(
                name: "Round",
                table: "WsWorkspacesReports",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "UIX_WorkspaceId_Round",
                table: "WsWorkspacesReports",
                columns: new[] { "WorkspaceId", "Round" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UIX_WorkspaceId_Round",
                table: "WsWorkspacesReports");

            migrationBuilder.AlterColumn<short>(
                name: "Round",
                table: "WsWorkspacesReports",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldDefaultValue: (short)1);

            migrationBuilder.CreateIndex(
                name: "IX_WsWorkspacesReports_WorkspaceId_Algorithm",
                table: "WsWorkspacesReports",
                columns: new[] { "WorkspaceId", "Algorithm" },
                unique: true);
        }
    }
}
