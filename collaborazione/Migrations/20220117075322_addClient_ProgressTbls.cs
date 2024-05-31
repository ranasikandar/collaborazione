using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace collaborazione.Migrations
{
    public partial class addClient_ProgressTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Progreses",
                columns: table => new
                {
                    ProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgressName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progreses", x => x.ProgressId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAddByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SurName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sale = table.Column<bool>(type: "bit", nullable: false),
                    Rent = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EstimatedCommitssion = table.Column<int>(type: "int", nullable: false),
                    ProgressItemId = table.Column<int>(type: "int", nullable: false),
                    ProgressDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_ClientAddByUserId",
                        column: x => x.ClientAddByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_Progreses_ProgressItemId",
                        column: x => x.ProgressItemId,
                        principalTable: "Progreses",
                        principalColumn: "ProgressId",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "Progreses",
                columns: new[] { "ProgressId", "ProgressName" },
                values: new object[,]
                {
                    { 1, "Submited by user" },
                    { 2, "Process by admin" },
                    { 3, "Eligible for Commitssion" },
                    { 4, "Commitssion Paid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientAddByUserId",
                table: "Clients",
                column: "ClientAddByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ProgressItemId",
                table: "Clients",
                column: "ProgressItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Progreses");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "772cda87-389e-443c-a7e1-05fb0ef7f67c",
                column: "ConcurrencyStamp",
                value: "742f06ba-6f32-462e-a5fd-7323e4d25a9e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "972cda86-389e-443c-a9e1-06fb0ef7f62e",
                column: "ConcurrencyStamp",
                value: "866cef42-f048-4342-aca2-6f600be10446");
        }
    }
}
