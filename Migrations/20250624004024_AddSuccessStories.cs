using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacesHunter.Migrations
{
    /// <inheritdoc />
    public partial class AddSuccessStories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "SuccessStories",
                newName: "DatePosted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DatePosted",
                table: "SuccessStories",
                newName: "Date");
        }
    }
}
