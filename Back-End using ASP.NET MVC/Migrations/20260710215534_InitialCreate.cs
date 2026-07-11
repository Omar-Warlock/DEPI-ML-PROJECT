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
                    player_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    club = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    country_of_birth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    country_of_citizenship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sub_position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    foot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    height_in_cm = table.Column<double>(type: "float", nullable: false),
                    international_caps = table.Column<double>(type: "float", nullable: false),
                    international_goals = table.Column<double>(type: "float", nullable: false),
                    age = table.Column<int>(type: "int", nullable: false),
                    log_market_value = table.Column<double>(type: "float", nullable: false),
                    contract_years_left = table.Column<double>(type: "float", nullable: false),
                    total_games = table.Column<double>(type: "float", nullable: false),
                    total_goals = table.Column<double>(type: "float", nullable: false),
                    total_assists = table.Column<double>(type: "float", nullable: false),
                    total_yellow = table.Column<double>(type: "float", nullable: false),
                    total_red = table.Column<double>(type: "float", nullable: false),
                    total_minutes = table.Column<double>(type: "float", nullable: false),
                    avg_minutes = table.Column<double>(type: "float", nullable: false),
                    goals_per_game = table.Column<double>(type: "float", nullable: false),
                    assists_per_game = table.Column<double>(type: "float", nullable: false),
                    num_transfers = table.Column<double>(type: "float", nullable: false),
                    total_transfer_fee = table.Column<double>(type: "float", nullable: false),
                    max_transfer_fee = table.Column<double>(type: "float", nullable: false),
                    avg_transfer_fee = table.Column<double>(type: "float", nullable: false),
                    goals_per_90 = table.Column<double>(type: "float", nullable: false),
                    assists_per_90 = table.Column<double>(type: "float", nullable: false),
                    goal_contributions = table.Column<double>(type: "float", nullable: false),
                    gc_per_90 = table.Column<double>(type: "float", nullable: false),
                    discipline_score = table.Column<double>(type: "float", nullable: false),
                    experience_score = table.Column<double>(type: "float", nullable: false),
                    intl_ratio = table.Column<double>(type: "float", nullable: false),
                    value_growth = table.Column<double>(type: "float", nullable: false),
                    is_international = table.Column<int>(type: "int", nullable: false),
                    position_enc = table.Column<int>(type: "int", nullable: false),
                    foot_enc = table.Column<int>(type: "int", nullable: false),
                    market_value_in_eur = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    highest_market_value_in_eur = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Predicted_Market_Value_EUR = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
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
