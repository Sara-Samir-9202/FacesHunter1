using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacesHunter.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "Persons",
                newName: "IsApproved");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Persons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReporterVerified",
                table: "Persons",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CreatedByUserId",
                table: "Persons",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Users_CreatedByUserId",
                table: "Persons",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Users_CreatedByUserId",
                table: "Persons");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Persons_CreatedByUserId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ReporterVerified",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "Persons",
                newName: "IsVerified");
        }
    }
}
