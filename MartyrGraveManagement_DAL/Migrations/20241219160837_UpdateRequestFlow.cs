using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestImage");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "RequestCustomer");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "RequestCustomer",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "VideoFile",
                table: "ReportGraves",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ReportGraves",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)");

            migrationBuilder.CreateTable(
                name: "Request_Material",
                columns: table => new
                {
                    RequestMaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_Material", x => x.RequestMaterialId);
                    table.ForeignKey(
                        name: "FK_Request_Material_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_Material_RequestCustomer_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestCustomer",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestNoteHistory",
                columns: table => new
                {
                    NoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestNoteHistory", x => x.NoteId);
                    table.ForeignKey(
                        name: "FK_RequestNoteHistory_RequestCustomer_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestCustomer",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestTask",
                columns: table => new
                {
                    RequestTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    ImageWorkSpace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTask", x => x.RequestTaskId);
                    table.ForeignKey(
                        name: "FK_RequestTask_Accounts_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestTask_RequestCustomer_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestCustomer",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestTaskImage",
                columns: table => new
                {
                    RequestTaskImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestTaskId = table.Column<int>(type: "int", nullable: false),
                    ImageRequestTaskCustomer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTaskImage", x => x.RequestTaskImageId);
                    table.ForeignKey(
                        name: "FK_RequestTaskImage_RequestTask_RequestTaskId",
                        column: x => x.RequestTaskId,
                        principalTable: "RequestTask",
                        principalColumn: "RequestTaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_Material_MaterialId",
                table: "Request_Material",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_Material_RequestId",
                table: "Request_Material",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestNoteHistory_RequestId",
                table: "RequestNoteHistory",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTask_RequestId",
                table: "RequestTask",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestTask_StaffId",
                table: "RequestTask",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTaskImage_RequestTaskId",
                table: "RequestTaskImage",
                column: "RequestTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_Material");

            migrationBuilder.DropTable(
                name: "RequestNoteHistory");

            migrationBuilder.DropTable(
                name: "RequestTaskImage");

            migrationBuilder.DropTable(
                name: "RequestTask");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "RequestCustomer",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "RequestCustomer",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoFile",
                table: "ReportGraves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ReportGraves",
                type: "nvarchar(500)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RequestImage",
                columns: table => new
                {
                    RequestImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageRequestCustomer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestImage", x => x.RequestImageId);
                    table.ForeignKey(
                        name: "FK_RequestImage_RequestCustomer_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestCustomer",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestImage_RequestId",
                table: "RequestImage",
                column: "RequestId");
        }
    }
}
