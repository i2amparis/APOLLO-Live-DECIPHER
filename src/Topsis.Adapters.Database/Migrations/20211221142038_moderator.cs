using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class moderator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7A",
                column: "ConcurrencyStamp",
                value: "af0daee0-9969-4a51-903f-0e39e6bdcc41");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7B",
                column: "ConcurrencyStamp",
                value: "25697d53-44fa-4861-a489-bc0112826a9d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7C",
                column: "ConcurrencyStamp",
                value: "1a21cdfd-2de5-4dfc-883e-51b9b39e76f2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA70",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "87727574-6a26-4124-a118-56520660bab7", "AQAAAAEAACcQAAAAEHtQ2W31aXIPp2EECMtb6aZTMmy5c6pgI6oAoM3Bn1+DH/7fDkBOaP0tE0r0+xWexQ==", "5f0367a8-348f-46aa-b900-b7dc968d8983" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CountryId", "Email", "EmailConfirmed", "FirstName", "JobCategoryId", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA71", 0, "c3da6f80-7506-45c6-8478-820c876db13b", null, "kkoasidis@epu.ntua.gr", true, null, null, null, false, null, "kkoasidis@epu.ntua.gr", "kkoasidis@epu.ntua.gr", "AQAAAAEAACcQAAAAEDkScCIrRtV+IVS4CbiwQy2alU0hSlaRLUeIfZHFTtW8HPsAD/f55TPhajXxHx2DpQ==", "", true, "80371d77-dfbf-44be-8efd-1b4d2dcc0747", false, "kkoasidis@epu.ntua.gr" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA71", "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7B" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA71", "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7C" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA71", "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7B" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA71", "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7C" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA71");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7A",
                column: "ConcurrencyStamp",
                value: "d88c0309-8220-4b8b-bed8-c3fad4f31032");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7B",
                column: "ConcurrencyStamp",
                value: "81af51b3-0585-4657-9970-c787e11eb5c1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7C",
                column: "ConcurrencyStamp",
                value: "2ee994ab-f1bb-4bce-8851-7818defdda9b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA70",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5a707ad6-bd1c-4706-8323-bef16250ed1a", "AQAAAAEAACcQAAAAEMMXtIDR5MY0QGKhlh5KxqWZ6YC1KkCnsh3ILk2xoKd6eKhbDB/T61n9kEoV7RRbIQ==", "7f0086e4-7718-491c-8627-18cdc291d491" });
        }
    }
}
