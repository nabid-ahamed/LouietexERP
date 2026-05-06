using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LouietexERP.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewPhoneNumber",
                table: "ProfileRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewProfilePicturePath",
                table: "ProfileRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedByUserId",
                table: "ProfileRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "ProfileRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRequests_ProcessedByUserId",
                table: "ProfileRequests",
                column: "ProcessedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileRequests_AspNetUsers_ProcessedByUserId",
                table: "ProfileRequests",
                column: "ProcessedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileRequests_AspNetUsers_ProcessedByUserId",
                table: "ProfileRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProfileRequests_ProcessedByUserId",
                table: "ProfileRequests");

            migrationBuilder.DropColumn(
                name: "NewPhoneNumber",
                table: "ProfileRequests");

            migrationBuilder.DropColumn(
                name: "NewProfilePicturePath",
                table: "ProfileRequests");

            migrationBuilder.DropColumn(
                name: "ProcessedByUserId",
                table: "ProfileRequests");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "ProfileRequests");
        }
    }
}
