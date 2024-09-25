using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCostraintAccountArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Areas_AreaId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_AreaId",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Accounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AreaId",
                table: "Accounts",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Areas_AreaId",
                table: "Accounts",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "AreaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
