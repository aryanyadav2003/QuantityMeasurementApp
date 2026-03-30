using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurementApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Operand1Value = table.Column<double>(type: "float", nullable: false),
                    Operand1Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Operand1MeasurementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Operand2Value = table.Column<double>(type: "float", nullable: true),
                    Operand2Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Operand2MeasurementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ResultValue = table.Column<double>(type: "float", nullable: true),
                    ResultUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResultMeasurementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ComparisonResult = table.Column<bool>(type: "bit", nullable: false),
                    ScalarResult = table.Column<double>(type: "float", nullable: false),
                    HasError = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_MeasurementType",
                table: "Measurements",
                column: "Operand1MeasurementType");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_Operation",
                table: "Measurements",
                column: "Operation");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_Timestamp",
                table: "Measurements",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurements");
        }
    }
}
