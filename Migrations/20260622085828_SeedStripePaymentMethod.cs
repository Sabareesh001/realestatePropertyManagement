using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class SeedStripePaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "payment_methods",
                columns: new[] { "id", "category", "created_at", "deleted_at", "description", "name", "updated_at" },
                values: new object[] { 7, "Online", new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Stripe card / digital wallet payment", "Stripe", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 7);
        }
    }
}
