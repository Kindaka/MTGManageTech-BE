using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_HistoricalEvents_HistoryId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalImages_HistoricalEvents_HistoryId",
                table: "HistoricalImages");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalRelatedMartyrs_HistoricalEvents_HistoryId",
                table: "HistoricalRelatedMartyrs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricalEvents",
                table: "HistoricalEvents");

            migrationBuilder.RenameTable(
                name: "HistoricalEvents",
                newName: "HistoricalEvent");

            migrationBuilder.RenameColumn(
                name: "HistoryId",
                table: "HistoricalRelatedMartyrs",
                newName: "BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricalRelatedMartyrs_HistoryId",
                table: "HistoricalRelatedMartyrs",
                newName: "IX_HistoricalRelatedMartyrs_BlogId");

            migrationBuilder.RenameColumn(
                name: "HistoryId",
                table: "HistoricalImages",
                newName: "BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricalImages_HistoryId",
                table: "HistoricalImages",
                newName: "IX_HistoricalImages_BlogId");

            migrationBuilder.RenameColumn(
                name: "HistoryId",
                table: "Comments",
                newName: "BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_HistoryId",
                table: "Comments",
                newName: "IX_Comments_BlogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricalEvent",
                table: "HistoricalEvent",
                column: "HistoryId");

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    BlogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    BlogName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    BlogContent = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.BlogId);
                    table.ForeignKey(
                        name: "FK_Blogs_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Blogs_HistoricalEvent_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "HistoricalEvent",
                        principalColumn: "HistoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_AccountId",
                table: "Blogs",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_HistoryId",
                table: "Blogs",
                column: "HistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                table: "Comments",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalImages_Blogs_BlogId",
                table: "HistoricalImages",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalRelatedMartyrs_Blogs_BlogId",
                table: "HistoricalRelatedMartyrs",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalImages_Blogs_BlogId",
                table: "HistoricalImages");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalRelatedMartyrs_Blogs_BlogId",
                table: "HistoricalRelatedMartyrs");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricalEvent",
                table: "HistoricalEvent");

            migrationBuilder.RenameTable(
                name: "HistoricalEvent",
                newName: "HistoricalEvents");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                table: "HistoricalRelatedMartyrs",
                newName: "HistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricalRelatedMartyrs_BlogId",
                table: "HistoricalRelatedMartyrs",
                newName: "IX_HistoricalRelatedMartyrs_HistoryId");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                table: "HistoricalImages",
                newName: "HistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricalImages_BlogId",
                table: "HistoricalImages",
                newName: "IX_HistoricalImages_HistoryId");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                table: "Comments",
                newName: "HistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_BlogId",
                table: "Comments",
                newName: "IX_Comments_HistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricalEvents",
                table: "HistoricalEvents",
                column: "HistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_HistoricalEvents_HistoryId",
                table: "Comments",
                column: "HistoryId",
                principalTable: "HistoricalEvents",
                principalColumn: "HistoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalImages_HistoricalEvents_HistoryId",
                table: "HistoricalImages",
                column: "HistoryId",
                principalTable: "HistoricalEvents",
                principalColumn: "HistoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalRelatedMartyrs_HistoricalEvents_HistoryId",
                table: "HistoricalRelatedMartyrs",
                column: "HistoryId",
                principalTable: "HistoricalEvents",
                principalColumn: "HistoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
