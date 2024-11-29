using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameTableHistoricalEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_HistoricalEvent_HistoryId",
                table: "Blogs");

            migrationBuilder.DropTable(
                name: "HistoricalEvent");

            migrationBuilder.CreateTable(
                name: "BlogCategory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogCategoryName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategory", x => x.HistoryId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_BlogCategory_HistoryId",
                table: "Blogs",
                column: "HistoryId",
                principalTable: "BlogCategory",
                principalColumn: "HistoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_BlogCategory_HistoryId",
                table: "Blogs");

            migrationBuilder.DropTable(
                name: "BlogCategory");

            migrationBuilder.CreateTable(
                name: "HistoricalEvent",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HistoryEventName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalEvent", x => x.HistoryId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_HistoricalEvent_HistoryId",
                table: "Blogs",
                column: "HistoryId",
                principalTable: "HistoricalEvent",
                principalColumn: "HistoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
