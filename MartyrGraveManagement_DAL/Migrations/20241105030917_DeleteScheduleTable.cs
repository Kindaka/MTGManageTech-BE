using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteScheduleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Schedule_ScheduleId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDetail_Schedule_ScheduleId",
                table: "ScheduleDetail");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "ScheduleDetail",
                newName: "SlotId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleDetail_ScheduleId",
                table: "ScheduleDetail",
                newName: "IX_ScheduleDetail_SlotId");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "Attendances",
                newName: "SlotId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_ScheduleId",
                table: "Attendances",
                newName: "IX_Attendances_SlotId");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "ScheduleDetail",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ScheduleDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "ScheduleDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Attendances",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "ImagePath1",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath2",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath3",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Slots_SlotId",
                table: "Attendances",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDetail_Slots_SlotId",
                table: "ScheduleDetail",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Slots_SlotId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDetail_Slots_SlotId",
                table: "ScheduleDetail");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ScheduleDetail");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ScheduleDetail");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "ScheduleDetail");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ImagePath1",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ImagePath2",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ImagePath3",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "SlotId",
                table: "ScheduleDetail",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleDetail_SlotId",
                table: "ScheduleDetail",
                newName: "IX_ScheduleDetail_ScheduleId");

            migrationBuilder.RenameColumn(
                name: "SlotId",
                table: "Attendances",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_SlotId",
                table: "Attendances",
                newName: "IX_Attendances_ScheduleId");

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_Schedule_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_SlotId",
                table: "Schedule",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Schedule_ScheduleId",
                table: "Attendances",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDetail_Schedule_ScheduleId",
                table: "ScheduleDetail",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
