using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisStats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnhancedMatchStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Player1FirstServePointsTotal",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player1FirstServesIn",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player1FirstServesTotal",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player1SecondServePointsTotal",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player2FirstServePointsTotal",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player2FirstServesIn",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player2FirstServesTotal",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player2SecondServePointsTotal",
                table: "MatchStatistics",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1FirstServePointsTotal",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "Player1FirstServesIn",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "Player1FirstServesTotal",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "Player1SecondServePointsTotal",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "Player2FirstServePointsTotal",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "Player2FirstServesIn",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "Player2FirstServesTotal",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "Player2SecondServePointsTotal",
                table: "MatchStatistics");
        }
    }
}
