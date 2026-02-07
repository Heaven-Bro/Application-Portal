using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChairmanAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_equipment_Condition",
                table: "equipment");

            migrationBuilder.DropIndex(
                name: "IX_equipment_EquipmentCode",
                table: "equipment");

            migrationBuilder.DropIndex(
                name: "IX_equipment_IsAvailable",
                table: "equipment");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptional",
                table: "service_steps",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                table: "service_steps",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "applications",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chairman_availability",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chairman_availability", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chairman_availability");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                table: "service_steps");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "applications");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptional",
                table: "service_steps",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_Condition",
                table: "equipment",
                column: "Condition");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_EquipmentCode",
                table: "equipment",
                column: "EquipmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_IsAvailable",
                table: "equipment",
                column: "IsAvailable");
        }
    }
}
