using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoutIQ.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    short_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    age = table.Column<int>(type: "int", nullable: false),
                    overall = table.Column<double>(type: "float", nullable: false),
                    potential = table.Column<double>(type: "float", nullable: false),
                    age_potential_gap = table.Column<double>(type: "float", nullable: false),
                    pace = table.Column<double>(type: "float", nullable: false),
                    shooting = table.Column<double>(type: "float", nullable: false),
                    passing = table.Column<double>(type: "float", nullable: false),
                    dribbling = table.Column<double>(type: "float", nullable: false),
                    defending = table.Column<double>(type: "float", nullable: false),
                    physic = table.Column<double>(type: "float", nullable: false),
                    position_group_GK = table.Column<int>(type: "int", nullable: false),
                    position_group_MID = table.Column<int>(type: "int", nullable: false),
                    position_group_ATT = table.Column<int>(type: "int", nullable: false),
                    market_value_eur = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    value_per_rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
