using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDocumentsAndEquipmentEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentAssignments",
                table: "EquipmentAssignments");

            migrationBuilder.RenameTable(
                name: "EquipmentAssignments",
                newName: "equipmentassignments");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "equipment",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_equipmentassignments",
                table: "equipmentassignments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "user_documents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: true),
                    FileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginalFileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_documents", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "step_submission_documents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StepSubmissionId = table.Column<long>(type: "bigint", nullable: false),
                    UserDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_step_submission_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_step_submission_documents_step_submissions_StepSubmissionId",
                        column: x => x.StepSubmissionId,
                        principalTable: "step_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_step_submission_documents_user_documents_UserDocumentId",
                        column: x => x.UserDocumentId,
                        principalTable: "user_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_equipmentassignments_ApplicationId",
                table: "equipmentassignments",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_equipmentassignments_EquipmentId",
                table: "equipmentassignments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_equipmentassignments_Status",
                table: "equipmentassignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_Condition",
                table: "equipment",
                column: "Condition");

            migrationBuilder.CreateIndex(
                name: "IX_step_submission_documents_StepSubmissionId",
                table: "step_submission_documents",
                column: "StepSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_step_submission_documents_UserDocumentId",
                table: "step_submission_documents",
                column: "UserDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_user_documents_ApplicationId",
                table: "user_documents",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_user_documents_IsDeleted",
                table: "user_documents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_user_documents_UserId",
                table: "user_documents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "step_submission_documents");

            migrationBuilder.DropTable(
                name: "user_documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_equipmentassignments",
                table: "equipmentassignments");

            migrationBuilder.DropIndex(
                name: "IX_equipmentassignments_ApplicationId",
                table: "equipmentassignments");

            migrationBuilder.DropIndex(
                name: "IX_equipmentassignments_EquipmentId",
                table: "equipmentassignments");

            migrationBuilder.DropIndex(
                name: "IX_equipmentassignments_Status",
                table: "equipmentassignments");

            migrationBuilder.DropIndex(
                name: "IX_equipment_Condition",
                table: "equipment");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "equipment");

            migrationBuilder.RenameTable(
                name: "equipmentassignments",
                newName: "EquipmentAssignments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentAssignments",
                table: "EquipmentAssignments",
                column: "Id");
        }
    }
}
