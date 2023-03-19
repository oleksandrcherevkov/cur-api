using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanTemplate.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBikeStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bikes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bikes");
        }
    }
}
