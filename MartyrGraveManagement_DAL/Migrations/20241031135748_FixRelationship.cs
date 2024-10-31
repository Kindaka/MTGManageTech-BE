using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalRelatedMartyrs_MartyrGraves_MartyrGraveMartyrId",
                table: "HistoricalRelatedMartyrs");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_HistoricalRelatedMartyrs_MartyrGraveMartyrId",
                table: "HistoricalRelatedMartyrs");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MartyrGraveMartyrId",
                table: "HistoricalRelatedMartyrs");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Orders",
                newName: "ExpectedCompletionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks",
                column: "DetailId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ExpectedCompletionDate",
                table: "Orders",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MartyrGraveMartyrId",
                table: "HistoricalRelatedMartyrs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalRelatedMartyrs_MartyrGraveMartyrId",
                table: "HistoricalRelatedMartyrs",
                column: "MartyrGraveMartyrId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalRelatedMartyrs_MartyrGraves_MartyrGraveMartyrId",
                table: "HistoricalRelatedMartyrs",
                column: "MartyrGraveMartyrId",
                principalTable: "MartyrGraves",
                principalColumn: "MartyrId");
        }
    }
}
