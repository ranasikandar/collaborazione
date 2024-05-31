using Microsoft.EntityFrameworkCore.Migrations;

namespace collaborazione.Migrations
{
    public partial class updUsrAddInviteBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "772cda87-389e-443c-a7e1-05fb0ef7f67c",
                column: "ConcurrencyStamp",
                value: "5b01dcce-1119-4799-b009-c3a12e51b507");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "972cda86-389e-443c-a9e1-06fb0ef7f62e",
                column: "ConcurrencyStamp",
                value: "b37b1674-fd7f-48e1-ba3d-3d1c254ddaf1");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 1,
                column: "ProgressName",
                value: "Da contattare");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 2,
                column: "ProgressName",
                value: "In elaborazione");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 3,
                column: "ProgressName",
                value: "Confermata");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 4,
                column: "ProgressName",
                value: "Commissione pagata");

            migrationBuilder.InsertData(
                table: "Progreses",
                columns: new[] { "ProgressId", "ProgressName" },
                values: new object[,]
                {
                    { 5, "Calcolo commissione" },
                    { 6, "Da ricevere profitto" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "InvitedBy",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "772cda87-389e-443c-a7e1-05fb0ef7f67c",
                column: "ConcurrencyStamp",
                value: "0c4cde6c-72dd-45a0-8a57-a98e589574fa");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "972cda86-389e-443c-a9e1-06fb0ef7f62e",
                column: "ConcurrencyStamp",
                value: "89c2f6b0-d45b-4375-9c31-ed980baac81a");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 1,
                column: "ProgressName",
                value: "Submited by user");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 2,
                column: "ProgressName",
                value: "Process by admin");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 3,
                column: "ProgressName",
                value: "Eligible for Commitssion");

            migrationBuilder.UpdateData(
                table: "Progreses",
                keyColumn: "ProgressId",
                keyValue: 4,
                column: "ProgressName",
                value: "Commitssion Paid");
        }
    }
}
