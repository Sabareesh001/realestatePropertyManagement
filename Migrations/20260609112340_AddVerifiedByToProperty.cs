using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddVerifiedByToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "verified_by",
                table: "properties",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_properties_verified_by",
                table: "properties",
                column: "verified_by");

            migrationBuilder.AddForeignKey(
                name: "properties_verified_by_fkey",
                table: "properties",
                column: "verified_by",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "properties_verified_by_fkey",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "IX_properties_verified_by",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "verified_by",
                table: "properties");
        }
    }
}
