using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyReportGraves");

            migrationBuilder.DropColumn(
                name: "AssignmentTaskId",
                table: "ScheduleDetail");

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
                name: "ScheduleDetailType",
                table: "ScheduleDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RequestType",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    TypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestType", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "RequestCustomer",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    MartyrId = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCustomer", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_RequestCustomer_Accounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCustomer_MartyrGraves_MartyrId",
                        column: x => x.MartyrId,
                        principalTable: "MartyrGraves",
                        principalColumn: "MartyrId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCustomer_RequestType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "RequestType",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportGraves",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    VideoFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportGraves", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_ReportGraves_RequestCustomer_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestCustomer",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportImage",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    UrlPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportImage", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ReportImage_ReportGraves_ReportId",
                        column: x => x.ReportId,
                        principalTable: "ReportGraves",
                        principalColumn: "ReportId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportGraves_RequestId",
                table: "ReportGraves",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportImage_ReportId",
                table: "ReportImage",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCustomer_CustomerId",
                table: "RequestCustomer",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCustomer_MartyrId",
                table: "RequestCustomer",
                column: "MartyrId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCustomer_TypeId",
                table: "RequestCustomer",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportImage");

            migrationBuilder.DropTable(
                name: "ReportGraves");

            migrationBuilder.DropTable(
                name: "RequestCustomer");

            migrationBuilder.DropTable(
                name: "RequestType");

            migrationBuilder.DropColumn(
                name: "ScheduleDetailType",
                table: "ScheduleDetail");

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

            migrationBuilder.CreateTable(
                name: "WeeklyReportGraves",
                columns: table => new
                {
                    WeeklyReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MartyrId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    DisciplinePoint = table.Column<int>(type: "int", nullable: false),
                    QualityOfFlowerPoint = table.Column<int>(type: "int", nullable: false),
                    QualityOfTotalGravePoint = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyReportGraves", x => x.WeeklyReportId);
                    table.ForeignKey(
                        name: "FK_WeeklyReportGraves_MartyrGraves_MartyrId",
                        column: x => x.MartyrId,
                        principalTable: "MartyrGraves",
                        principalColumn: "MartyrId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyReportGraves_MartyrId",
                table: "WeeklyReportGraves",
                column: "MartyrId");
        }
    }
}
