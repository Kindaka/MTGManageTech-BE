using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTableFeedbackForAssignTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkTo",
                table: "Notifications",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssignmentTask_Feedback",
                columns: table => new
                {
                    AssignmentTaskFeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AssignmentTaskId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    ResponseContent = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTask_Feedback", x => x.AssignmentTaskFeedbackId);
                    table.ForeignKey(
                        name: "FK_AssignmentTask_Feedback_AssignmentTask_AssignmentTaskId",
                        column: x => x.AssignmentTaskId,
                        principalTable: "AssignmentTask",
                        principalColumn: "AssignmentTaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTask_Feedback_AssignmentTaskId",
                table: "AssignmentTask_Feedback",
                column: "AssignmentTaskId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentTask_Feedback");

            migrationBuilder.DropColumn(
                name: "LinkTo",
                table: "Notifications");
        }
    }
}
