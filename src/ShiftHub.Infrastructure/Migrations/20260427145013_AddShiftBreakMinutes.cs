using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftBreakMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreakMinutes",
                table: "Shifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakMinutes",
                table: "Shifts");
        }
    }
}
