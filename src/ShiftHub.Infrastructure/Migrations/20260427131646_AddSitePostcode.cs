using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSitePostcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "Sites",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "Sites");
        }
    }
}
