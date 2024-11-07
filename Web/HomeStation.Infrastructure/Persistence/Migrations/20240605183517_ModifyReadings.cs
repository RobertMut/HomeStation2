using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeStation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reading_Day",
                table: "Climate");

            migrationBuilder.DropColumn(
                name: "Reading_Month",
                table: "Climate");

            migrationBuilder.DropColumn(
                name: "Reading_Week",
                table: "Climate");

            migrationBuilder.DropColumn(
                name: "Reading_Day",
                table: "AirQuality");

            migrationBuilder.DropColumn(
                name: "Reading_Month",
                table: "AirQuality");

            migrationBuilder.DropColumn(
                name: "Reading_Week",
                table: "AirQuality");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reading_Day",
                table: "Climate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reading_Month",
                table: "Climate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reading_Week",
                table: "Climate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reading_Day",
                table: "AirQuality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reading_Month",
                table: "AirQuality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reading_Week",
                table: "AirQuality",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
