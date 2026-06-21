using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddChargePaymentSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "charge_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PENDING", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PARTIALLY_PAID", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PAID", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OVERDUE", null },
                    { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CANCELLED", null }
                });

            migrationBuilder.InsertData(
                table: "charge_types",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Monthly Rent", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security Deposit", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Upfront Payment", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance", null },
                    { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Penalty", null },
                    { 6, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other", null }
                });

            migrationBuilder.InsertData(
                table: "currencies",
                columns: new[] { "id", "code", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[] { 1, "INR", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Indian Rupee", null });

            migrationBuilder.InsertData(
                table: "payment_methods",
                columns: new[] { "id", "category", "created_at", "deleted_at", "description", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, "Offline", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Cash", null },
                    { 2, "Online", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Bank Transfer", null },
                    { 3, "Online", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "UPI", null },
                    { 4, "Online", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Credit Card", null },
                    { 5, "Online", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Debit Card", null },
                    { 6, "Offline", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Cheque", null }
                });

            migrationBuilder.InsertData(
                table: "payment_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PENDING", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COMPLETED", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "FAILED", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REFUNDED", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "charge_statuses",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "charge_statuses",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "charge_statuses",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "charge_statuses",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "charge_statuses",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "charge_types",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "charge_types",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "charge_types",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "charge_types",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "charge_types",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "charge_types",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "currencies",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "payment_statuses",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "payment_statuses",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "payment_statuses",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "payment_statuses",
                keyColumn: "id",
                keyValue: 4);
        }
    }
}
