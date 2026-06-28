using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoutIQ.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "is_at_peak",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "position_group_DEF",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_at_peak",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "position_group_DEF",
                table: "Players");
        }
    }
}
