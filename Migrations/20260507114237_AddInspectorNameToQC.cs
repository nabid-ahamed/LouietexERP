using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LouietexERP.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectorNameToQC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InspectorName",
                table: "QCInspections",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectorName",
                table: "QCInspections");
        }
    }
}
