using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipLocationAndMatyrGrave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MartyrGraves_LocationId",
                table: "MartyrGraves");

            migrationBuilder.CreateIndex(
                name: "IX_MartyrGraves_LocationId",
                table: "MartyrGraves",
                column: "LocationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MartyrGraves_LocationId",
                table: "MartyrGraves");

            migrationBuilder.CreateIndex(
                name: "IX_MartyrGraves_LocationId",
                table: "MartyrGraves",
                column: "LocationId");
        }
    }
}
