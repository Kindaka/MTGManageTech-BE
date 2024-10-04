using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class SetOrderIdSeedValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DBCC CHECKIDENT ('Orders', RESEED, 199);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DBCC CHECKIDENT ('Orders', RESEED, 0);");
        }
    }
}
