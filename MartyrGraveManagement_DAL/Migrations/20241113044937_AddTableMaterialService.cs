using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMaterialService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Services_ServiceId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_ServiceId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Materials");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Materials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Material_Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Material_Services_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_Services_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Material_Services_MaterialId",
                table: "Material_Services",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_Services_ServiceId",
                table: "Material_Services",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Material_Services");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Materials");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_ServiceId",
                table: "Materials",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Services_ServiceId",
                table: "Materials",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
