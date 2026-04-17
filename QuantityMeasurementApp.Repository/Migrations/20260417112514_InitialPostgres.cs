using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuantityMeasurementApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuantityMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Operand1Value = table.Column<double>(type: "double precision", nullable: false),
                    Operand1Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operand1MeasurementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operand2Value = table.Column<double>(type: "double precision", nullable: true),
                    Operand2Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Operand2MeasurementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Operation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResultValue = table.Column<double>(type: "double precision", nullable: true),
                    ResultUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultMeasurementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ComparisonResult = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ScalarResult = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    HasError = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityMeasurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_MeasurementType",
                table: "QuantityMeasurements",
                column: "Operand1MeasurementType");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_Operation",
                table: "QuantityMeasurements",
                column: "Operation");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_Timestamp",
                table: "QuantityMeasurements",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuantityMeasurements");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
