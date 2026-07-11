using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoutIQ.Migrations
{
    /// <inheritdoc />
    public partial class FixPositionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE Players ALTER COLUMN position int NOT NULL"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE Players ALTER COLUMN position nvarchar(max) NOT NULL"
            );
        }
    }
}
