using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCartQuantityEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartQuantity",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartQuantity",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
