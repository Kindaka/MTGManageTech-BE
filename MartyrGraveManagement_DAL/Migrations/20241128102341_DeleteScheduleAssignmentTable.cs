using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteScheduleAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentTask_Schedule_Assignment_AssignmentId",
                table: "AssignmentTask");

            migrationBuilder.DropTable(
                name: "Schedule_Assignment");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "AssignmentTask",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentTask_AssignmentId",
                table: "AssignmentTask",
                newName: "IX_AssignmentTask_StaffId");

            migrationBuilder.AddColumn<int>(
                name: "ServiceScheduleId",
                table: "AssignmentTask",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTask_ServiceScheduleId",
                table: "AssignmentTask",
                column: "ServiceScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTask_Accounts_StaffId",
                table: "AssignmentTask",
                column: "StaffId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTask_Service_Schedule_ServiceScheduleId",
                table: "AssignmentTask",
                column: "ServiceScheduleId",
                principalTable: "Service_Schedule",
                principalColumn: "ServiceScheduleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentTask_Accounts_StaffId",
                table: "AssignmentTask");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentTask_Service_Schedule_ServiceScheduleId",
                table: "AssignmentTask");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentTask_ServiceScheduleId",
                table: "AssignmentTask");

            migrationBuilder.DropColumn(
                name: "ServiceScheduleId",
                table: "AssignmentTask");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "AssignmentTask",
                newName: "AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentTask_StaffId",
                table: "AssignmentTask",
                newName: "IX_AssignmentTask_AssignmentId");

            migrationBuilder.CreateTable(
                name: "Schedule_Assignment",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceScheduleId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule_Assignment", x => x.AssignmentId);
                    table.ForeignKey(
                        name: "FK_Schedule_Assignment_Accounts_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedule_Assignment_Service_Schedule_ServiceScheduleId",
                        column: x => x.ServiceScheduleId,
                        principalTable: "Service_Schedule",
                        principalColumn: "ServiceScheduleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_Assignment_ServiceScheduleId",
                table: "Schedule_Assignment",
                column: "ServiceScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_Assignment_StaffId",
                table: "Schedule_Assignment",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTask_Schedule_Assignment_AssignmentId",
                table: "AssignmentTask",
                column: "AssignmentId",
                principalTable: "Schedule_Assignment",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
