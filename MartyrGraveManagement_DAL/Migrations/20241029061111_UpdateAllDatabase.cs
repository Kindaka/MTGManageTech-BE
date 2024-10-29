using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAllDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Orders_OrderId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_GraveImage_MartyrGraves_MartyrId",
                table: "GraveImage");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_Services_ServiceId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Material",
                table: "Material");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GraveImage",
                table: "GraveImage");

            migrationBuilder.DropColumn(
                name: "AreaNumber",
                table: "MartyrGraves");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "MartyrGraves");

            migrationBuilder.RenameTable(
                name: "Material",
                newName: "Materials");

            migrationBuilder.RenameTable(
                name: "GraveImage",
                newName: "GraveImages");

            migrationBuilder.RenameColumn(
                name: "RowNumber",
                table: "MartyrGraves",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "MartyrNumber",
                table: "MartyrGraves",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Feedbacks",
                newName: "DetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_OrderId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_DetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Material_ServiceId",
                table: "Materials",
                newName: "IX_Materials_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_GraveImage_MartyrId",
                table: "GraveImages",
                newName: "IX_GraveImages_MartyrId");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "MartyrGraveInformations",
                type: "nvarchar(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Medal",
                table: "MartyrGraveInformations",
                type: "nvarchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonOfSacrifice",
                table: "MartyrGraveInformations",
                type: "nvarchar(1000)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Feedbacks",
                type: "nvarchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Materials",
                table: "Materials",
                column: "MaterialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraveImages",
                table: "GraveImages",
                column: "ImageId");

            migrationBuilder.CreateTable(
                name: "GraveServices",
                columns: table => new
                {
                    GraveServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MartyrId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraveServices", x => x.GraveServiceId);
                    table.ForeignKey(
                        name: "FK_GraveServices_MartyrGraves_MartyrId",
                        column: x => x.MartyrId,
                        principalTable: "MartyrGraves",
                        principalColumn: "MartyrId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GraveServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalEvents",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoryEventName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalEvents", x => x.HistoryId);
                });

            migrationBuilder.CreateTable(
                name: "HolidayEvents",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayEvents", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_HolidayEvents_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Icons",
                columns: table => new
                {
                    IconId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IconImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconName = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icons", x => x.IconId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    MartyrNumber = table.Column<int>(type: "int", nullable: false),
                    AreaNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    SlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.SlotId);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_HistoricalEvents_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "HistoricalEvents",
                        principalColumn: "HistoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_HistoricalImages_HistoricalEvents_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "HistoricalEvents",
                        principalColumn: "HistoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalRelatedMartyrs",
                columns: table => new
                {
                    RelatedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    InformationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    MartyrGraveMartyrId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalRelatedMartyrs", x => x.RelatedId);
                    table.ForeignKey(
                        name: "FK_HistoricalRelatedMartyrs_HistoricalEvents_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "HistoricalEvents",
                        principalColumn: "HistoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoricalRelatedMartyrs_MartyrGraveInformations_InformationId",
                        column: x => x.InformationId,
                        principalTable: "MartyrGraveInformations",
                        principalColumn: "InformationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoricalRelatedMartyrs_MartyrGraves_MartyrGraveMartyrId",
                        column: x => x.MartyrGraveMartyrId,
                        principalTable: "MartyrGraves",
                        principalColumn: "MartyrId");
                });

            migrationBuilder.CreateTable(
                name: "EventImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_EventImages_HolidayEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "HolidayEvents",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationAccounts_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "NotificationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleStaffs",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleStaffs", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_ScheduleStaffs_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleStaffs_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment_Icons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IconId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment_Icons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Icons_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_Icons_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Icons_Icons_IconId",
                        column: x => x.IconId,
                        principalTable: "Icons",
                        principalColumn: "IconId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentReports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_CommentReports_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentReports_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MartyrGraves_AccountId",
                table: "MartyrGraves",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MartyrGraves_LocationId",
                table: "MartyrGraves",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Icons_AccountId",
                table: "Comment_Icons",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Icons_CommentId",
                table: "Comment_Icons",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Icons_IconId",
                table: "Comment_Icons",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_AccountId",
                table: "CommentReports",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_CommentId",
                table: "CommentReports",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AccountId",
                table: "Comments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_HistoryId",
                table: "Comments",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EventImages_EventId",
                table: "EventImages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_GraveServices_MartyrId",
                table: "GraveServices",
                column: "MartyrId");

            migrationBuilder.CreateIndex(
                name: "IX_GraveServices_ServiceId",
                table: "GraveServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalImages_HistoryId",
                table: "HistoricalImages",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalRelatedMartyrs_HistoryId",
                table: "HistoricalRelatedMartyrs",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalRelatedMartyrs_InformationId",
                table: "HistoricalRelatedMartyrs",
                column: "InformationId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalRelatedMartyrs_MartyrGraveMartyrId",
                table: "HistoricalRelatedMartyrs",
                column: "MartyrGraveMartyrId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayEvents_AccountId",
                table: "HolidayEvents",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAccounts_AccountId",
                table: "NotificationAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAccounts_NotificationId",
                table: "NotificationAccounts",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleStaffs_AccountId",
                table: "ScheduleStaffs",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleStaffs_SlotId",
                table: "ScheduleStaffs",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_OrderDetails_DetailId",
                table: "Feedbacks",
                column: "DetailId",
                principalTable: "OrderDetails",
                principalColumn: "DetailId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GraveImages_MartyrGraves_MartyrId",
                table: "GraveImages",
                column: "MartyrId",
                principalTable: "MartyrGraves",
                principalColumn: "MartyrId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MartyrGraves_Accounts_AccountId",
                table: "MartyrGraves",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MartyrGraves_Locations_LocationId",
                table: "MartyrGraves",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Services_ServiceId",
                table: "Materials",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_OrderDetails_DetailId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_GraveImages_MartyrGraves_MartyrId",
                table: "GraveImages");

            migrationBuilder.DropForeignKey(
                name: "FK_MartyrGraves_Accounts_AccountId",
                table: "MartyrGraves");

            migrationBuilder.DropForeignKey(
                name: "FK_MartyrGraves_Locations_LocationId",
                table: "MartyrGraves");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Services_ServiceId",
                table: "Materials");

            migrationBuilder.DropTable(
                name: "Comment_Icons");

            migrationBuilder.DropTable(
                name: "CommentReports");

            migrationBuilder.DropTable(
                name: "EventImages");

            migrationBuilder.DropTable(
                name: "GraveServices");

            migrationBuilder.DropTable(
                name: "HistoricalImages");

            migrationBuilder.DropTable(
                name: "HistoricalRelatedMartyrs");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "NotificationAccounts");

            migrationBuilder.DropTable(
                name: "ScheduleStaffs");

            migrationBuilder.DropTable(
                name: "Icons");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "HolidayEvents");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "HistoricalEvents");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_MartyrGraves_AccountId",
                table: "MartyrGraves");

            migrationBuilder.DropIndex(
                name: "IX_MartyrGraves_LocationId",
                table: "MartyrGraves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Materials",
                table: "Materials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GraveImages",
                table: "GraveImages");

            migrationBuilder.DropColumn(
                name: "ReasonOfSacrifice",
                table: "MartyrGraveInformations");

            migrationBuilder.RenameTable(
                name: "Materials",
                newName: "Material");

            migrationBuilder.RenameTable(
                name: "GraveImages",
                newName: "GraveImage");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "MartyrGraves",
                newName: "RowNumber");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "MartyrGraves",
                newName: "MartyrNumber");

            migrationBuilder.RenameColumn(
                name: "DetailId",
                table: "Feedbacks",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_DetailId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_ServiceId",
                table: "Material",
                newName: "IX_Material_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_GraveImages_MartyrId",
                table: "GraveImage",
                newName: "IX_GraveImage_MartyrId");

            migrationBuilder.AddColumn<int>(
                name: "AreaNumber",
                table: "MartyrGraves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "MartyrGraves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "MartyrGraveInformations",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Medal",
                table: "MartyrGraveInformations",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Feedbacks",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Material",
                table: "Material",
                column: "MaterialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraveImage",
                table: "GraveImage",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks",
                column: "DetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Orders_OrderId",
                table: "Feedbacks",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GraveImage_MartyrGraves_MartyrId",
                table: "GraveImage",
                column: "MartyrId",
                principalTable: "MartyrGraves",
                principalColumn: "MartyrId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_Services_ServiceId",
                table: "Material",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
