using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToRemainingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "user_verifications",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "user_profiles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "tenant_profiles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "tenant_profiles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "tenant_profiles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "states",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "states",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "states",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "proposal_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "proposal_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "proposal_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "property_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "property_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "property_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "profile_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "profile_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "profile_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "payments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "payments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "payment_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "payment_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "payment_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "owner_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "owner_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "owner_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "owner_profiles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "owner_profiles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "owner_profiles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "leases",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "leases",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "leases",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "lease_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "lease_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "lease_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "lease_proposals",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "lease_proposals",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "documents",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "documents",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "document_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "document_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "document_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "districts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "districts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "districts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "currencies",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "currencies",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "currencies",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "countries",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "countries",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "countries",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "complaints",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "complaints",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "complaints",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "complaint_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "complaint_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "complaint_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "complaint_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "complaint_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "complaint_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "complaint_priorities",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "complaint_priorities",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "complaint_priorities",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "cities",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "cities",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "cities",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "charges",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "charges",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "charge_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "charge_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "charge_types",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "charge_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "charge_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "charge_statuses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "bank_accounts",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "user_verifications");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "tenant_profiles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "tenant_profiles");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "tenant_profiles");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "states");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "states");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "states");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "proposal_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "proposal_statuses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "proposal_statuses");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "property_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "property_statuses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "property_statuses");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "profile_types");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "profile_types");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "profile_types");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "payment_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "payment_statuses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "payment_statuses");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "owner_types");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "owner_types");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "owner_types");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "owner_profiles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "owner_profiles");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "owner_profiles");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "leases");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "leases");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "leases");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "lease_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "lease_statuses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "lease_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "lease_proposals");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "lease_proposals");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "document_types");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "document_types");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "document_types");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "districts");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "districts");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "districts");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "currencies");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "currencies");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "currencies");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "complaint_types");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "complaint_types");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "complaint_types");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "complaint_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "complaint_statuses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "complaint_statuses");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "complaint_priorities");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "complaint_priorities");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "complaint_priorities");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "cities");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "cities");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "cities");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "charges");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "charges");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "charge_types");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "charge_types");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "charge_types");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "charge_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "charge_statuses");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "charge_statuses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "bank_accounts");
        }
    }
}
