using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class SeedDocumentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "document_types",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Pan card", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Property Deed", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Salary Slip", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Lease Agreement", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "document_types",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "document_types",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "document_types",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "document_types",
                keyColumn: "id",
                keyValue: 4);
        }
    }
}
