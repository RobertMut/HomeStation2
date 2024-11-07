using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeStation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirQuality",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Pm2_5 = table.Column<int>(type: "int", nullable: false),
                    Pm10 = table.Column<int>(type: "int", nullable: false),
                    Pm1_0 = table.Column<int>(type: "int", nullable: false),
                    Reading_Month = table.Column<int>(type: "int", nullable: false),
                    Reading_Week = table.Column<int>(type: "int", nullable: false),
                    Reading_Day = table.Column<int>(type: "int", nullable: false),
                    Reading_Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirQuality", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirQuality_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Climate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<double>(type: "float", nullable: false),
                    Pressure = table.Column<double>(type: "float", nullable: false),
                    Reading_Month = table.Column<int>(type: "int", nullable: false),
                    Reading_Week = table.Column<int>(type: "int", nullable: false),
                    Reading_Day = table.Column<int>(type: "int", nullable: false),
                    Reading_Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Climate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Climate_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirQuality_DeviceId",
                table: "AirQuality",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Climate_DeviceId",
                table: "Climate",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirQuality");

            migrationBuilder.DropTable(
                name: "Climate");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
