using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LouietexERP.Migrations
{
    /// <inheritdoc />
    public partial class AddErpWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Productions;");

            migrationBuilder.RenameColumn(
                name: "ProductionDate",
                table: "Productions",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Productions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Productions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Productions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Productions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Productions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Supervisor",
                table: "Productions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QCInspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionId = table.Column<int>(type: "int", nullable: false),
                    CheckedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DefectCount = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QCStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QCInspections_AspNetUsers_CheckedByUserId",
                        column: x => x.CheckedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QCInspections_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productions_OrderId",
                table: "Productions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_QCInspections_CheckedByUserId",
                table: "QCInspections",
                column: "CheckedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QCInspections_ProductionId",
                table: "QCInspections",
                column: "ProductionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productions_Orders_OrderId",
                table: "Productions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productions_Orders_OrderId",
                table: "Productions");

            migrationBuilder.DropTable(
                name: "QCInspections");

            migrationBuilder.DropIndex(
                name: "IX_Productions_OrderId",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "Supervisor",
                table: "Productions");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Productions",
                newName: "ProductionDate");
        }
    }
}
