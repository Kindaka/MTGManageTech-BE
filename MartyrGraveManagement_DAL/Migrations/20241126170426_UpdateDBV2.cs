using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDetail_Slots_SlotId",
                table: "ScheduleDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDetail_Tasks_TaskId",
                table: "ScheduleDetail");

            // 1. Xóa khóa ngoại
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDetail_SlotId",
                table: "ScheduleDetail");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDetail_TaskId",
                table: "ScheduleDetail");


            migrationBuilder.DropColumn(
                name: "ImagePath1",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ImagePath2",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "ScheduleDetail");

            migrationBuilder.RenameColumn(
                name: "ImagePath3",
                table: "Tasks",
                newName: "ImageWorkSpace");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "RecurringType",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isScheduleService",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "ScheduleDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentTaskId",
                table: "ScheduleDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1000, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "OrderDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Materials",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfSacrifice",
                table: "MartyrGraveInformations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "Gender",
                table: "MartyrGraveInformations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // 4. Tạo lại khóa chính cho Orders
            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "OrderId");

            // 5. Tạo lại khóa ngoại từ OrderDetails
            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateTable(
                name: "CustomerWallet",
                columns: table => new
                {
                    WalletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerWallet", x => x.WalletId);
                    table.ForeignKey(
                        name: "FK_CustomerWallet_Accounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Service_Schedule",
                columns: table => new
                {
                    ServiceScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    MartyrId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScheduleDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DayOfMonth = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service_Schedule", x => x.ServiceScheduleId);
                    table.ForeignKey(
                        name: "FK_Service_Schedule_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_Schedule_MartyrGraves_MartyrId",
                        column: x => x.MartyrId,
                        principalTable: "MartyrGraves",
                        principalColumn: "MartyrId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_Schedule_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskImage",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    ImageWorkSpace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskImage", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_TaskImage_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionBalanceHistory",
                columns: table => new
                {
                    TransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BalanceAfterTransaction = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionBalanceHistory", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_TransactionBalanceHistory_Accounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "AssignmentTask",
                columns: table => new
                {
                    AssignmentTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    ImageWorkSpace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTask", x => x.AssignmentTaskId);
                    table.ForeignKey(
                        name: "FK_AssignmentTask_Schedule_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Schedule_Assignment",
                        principalColumn: "AssignmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentTaskImage",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentTaskId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTaskImage", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_AssignmentTaskImage_AssignmentTask_AssignmentTaskId",
                        column: x => x.AssignmentTaskId,
                        principalTable: "AssignmentTask",
                        principalColumn: "AssignmentTaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTask_AssignmentId",
                table: "AssignmentTask",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTaskImage_AssignmentTaskId",
                table: "AssignmentTaskImage",
                column: "AssignmentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerWallet_CustomerId",
                table: "CustomerWallet",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_Assignment_ServiceScheduleId",
                table: "Schedule_Assignment",
                column: "ServiceScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_Assignment_StaffId",
                table: "Schedule_Assignment",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Schedule_AccountId",
                table: "Service_Schedule",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Schedule_MartyrId",
                table: "Service_Schedule",
                column: "MartyrId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Schedule_ServiceId",
                table: "Service_Schedule",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskImage_TaskId",
                table: "TaskImage",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionBalanceHistory_CustomerId",
                table: "TransactionBalanceHistory",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentTaskImage");

            migrationBuilder.DropTable(
                name: "CustomerWallet");

            migrationBuilder.DropTable(
                name: "TaskImage");

            migrationBuilder.DropTable(
                name: "TransactionBalanceHistory");

            migrationBuilder.DropTable(
                name: "AssignmentTask");

            migrationBuilder.DropTable(
                name: "Schedule_Assignment");

            migrationBuilder.DropTable(
                name: "Service_Schedule");

            migrationBuilder.DropColumn(
                name: "RecurringType",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "isScheduleService",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AssignmentTaskId",
                table: "ScheduleDetail");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "MartyrGraveInformations");

            migrationBuilder.RenameColumn(
                name: "ImageWorkSpace",
                table: "Tasks",
                newName: "ImagePath3");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath1",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath2",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Services",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "ScheduleDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlotId",
                table: "ScheduleDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Materials",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfSacrifice",
                table: "MartyrGraveInformations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    SlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    SlotName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.SlotId);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ImagePath1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_Attendances_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendances_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_SlotId",
                table: "ScheduleDetail",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_TaskId",
                table: "ScheduleDetail",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AccountId",
                table: "Attendances",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SlotId",
                table: "Attendances",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDetail_Slots_SlotId",
                table: "ScheduleDetail",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDetail_Tasks_TaskId",
                table: "ScheduleDetail",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
