using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class SeedPropertyStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "property_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Draft", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Submitted", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Verified", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Rejected", null },
                    { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Available", null },
                    { 6, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Occupied", null },
                    { 7, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Unavailable", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "property_statuses",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "property_statuses",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "property_statuses",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "property_statuses",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "property_statuses",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "property_statuses",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "property_statuses",
                keyColumn: "id",
                keyValue: 7);
        }
    }
}
