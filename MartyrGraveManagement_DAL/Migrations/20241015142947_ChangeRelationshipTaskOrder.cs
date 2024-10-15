using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipTaskOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NameOfWork",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "UrlImage",
                table: "Tasks",
                newName: "ImagePath3");

            migrationBuilder.RenameColumn(
                name: "TypeOfWork",
                table: "Tasks",
                newName: "DetailId");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Tasks",
                newName: "ImagePath2");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath1",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks",
                column: "DetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_OrderDetails_DetailId",
                table: "Tasks",
                column: "DetailId",
                principalTable: "OrderDetails",
                principalColumn: "DetailId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_OrderDetails_DetailId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DetailId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ImagePath1",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ImagePath3",
                table: "Tasks",
                newName: "UrlImage");

            migrationBuilder.RenameColumn(
                name: "ImagePath2",
                table: "Tasks",
                newName: "ImagePath");

            migrationBuilder.RenameColumn(
                name: "DetailId",
                table: "Tasks",
                newName: "TypeOfWork");

            migrationBuilder.AddColumn<string>(
                name: "NameOfWork",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks",
                column: "OrderId",
                unique: true);
        }
    }
}
