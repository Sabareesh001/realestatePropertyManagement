using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeConnectFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "stripe_account_id",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "stripe_charges_enabled",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "stripe_details_submitted",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "stripe_payouts_enabled",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "platform_fee_amount",
                table: "payments",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "stripe_payment_intent_id",
                table: "payments",
                type: "character varying",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "stripe_transfer_id",
                table: "payments",
                type: "character varying",
                nullable: true);

            migrationBuilder.InsertData(
                table: "payment_methods",
                columns: new[] { "id", "category", "created_at", "deleted_at", "description", "name", "updated_at" },
                values: new object[] { 7, "Online", new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Stripe card / digital wallet payment", "Stripe", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stripe_account_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "stripe_charges_enabled",
                table: "users");

            migrationBuilder.DropColumn(
                name: "stripe_details_submitted",
                table: "users");

            migrationBuilder.DropColumn(
                name: "stripe_payouts_enabled",
                table: "users");

            migrationBuilder.DropColumn(
                name: "platform_fee_amount",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "stripe_payment_intent_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "stripe_transfer_id",
                table: "payments");

            migrationBuilder.DeleteData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 7);
        }
    }
}
