using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttendanceSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ScheduleStaffs_ScheduleId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleStaffs_Slots_SlotId",
                table: "ScheduleStaffs");

            migrationBuilder.DropTable(
                name: "ScheduleTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleStaffs",
                table: "ScheduleStaffs");

            migrationBuilder.RenameTable(
                name: "ScheduleStaffs",
                newName: "Schedule");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleStaffs_SlotId",
                table: "Schedule",
                newName: "IX_Schedule_SlotId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule",
                column: "ScheduleId");

            migrationBuilder.CreateTable(
                name: "ScheduleDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleDetail_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleDetail_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleDetail_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_AccountId",
                table: "ScheduleDetail",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_ScheduleId",
                table: "ScheduleDetail",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_TaskId",
                table: "ScheduleDetail",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Schedule_ScheduleId",
                table: "Attendances",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Slots_SlotId",
                table: "Schedule",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Schedule_ScheduleId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Slots_SlotId",
                table: "Schedule");

            migrationBuilder.DropTable(
                name: "ScheduleDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule");

            migrationBuilder.RenameTable(
                name: "Schedule",
                newName: "ScheduleStaffs");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_SlotId",
                table: "ScheduleStaffs",
                newName: "IX_ScheduleStaffs_SlotId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleStaffs",
                table: "ScheduleStaffs",
                column: "ScheduleId");

            migrationBuilder.CreateTable(
                name: "ScheduleTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleTasks_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleTasks_ScheduleStaffs_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "ScheduleStaffs",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTasks_AccountId",
                table: "ScheduleTasks",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTasks_ScheduleId",
                table: "ScheduleTasks",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTasks_TaskId",
                table: "ScheduleTasks",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ScheduleStaffs_ScheduleId",
                table: "Attendances",
                column: "ScheduleId",
                principalTable: "ScheduleStaffs",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleStaffs_Slots_SlotId",
                table: "ScheduleStaffs",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
