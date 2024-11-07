using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeStation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Devices");

            migrationBuilder.AddColumn<bool>(
                name: "IsKnown",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKnown",
                table: "Devices");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
