using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusesAndAddLeaseDocs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.AddColumn<Guid>(
                name: "agreement_document_id",
                table: "leases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "signed_agreement_document_id",
                table: "leases",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "document_types",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[] { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Signed Lease Agreement", null });

            migrationBuilder.InsertData(
                table: "lease_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "DRAFT", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUBMITTED", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PENDING_SIGNATURE", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "TENANT_SIGNED", null },
                    { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVE", null },
                    { 6, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REJECTED", null },
                    { 7, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "TERMINATED", null },
                    { 8, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "EXPIRED", null }
                });

            // Temporary renames to free up unique names
            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "TEMP_APPROVED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "TEMP_REJECTED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "TEMP_CANCELLED");

            // Final renames
            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "APPROVED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "REJECTED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "EXPIRED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "CANCELLED");

            migrationBuilder.CreateIndex(
                name: "IX_leases_agreement_document_id",
                table: "leases",
                column: "agreement_document_id");

            migrationBuilder.CreateIndex(
                name: "IX_leases_signed_agreement_document_id",
                table: "leases",
                column: "signed_agreement_document_id");

            migrationBuilder.AddForeignKey(
                name: "leases_agreement_document_id_fkey",
                table: "leases",
                column: "agreement_document_id",
                principalTable: "documents",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "leases_signed_agreement_document_id_fkey",
                table: "leases",
                column: "signed_agreement_document_id",
                principalTable: "documents",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "leases_agreement_document_id_fkey",
                table: "leases");

            migrationBuilder.DropForeignKey(
                name: "leases_signed_agreement_document_id_fkey",
                table: "leases");

            migrationBuilder.DropIndex(
                name: "IX_leases_agreement_document_id",
                table: "leases");

            migrationBuilder.DropIndex(
                name: "IX_leases_signed_agreement_document_id",
                table: "leases");

            migrationBuilder.DeleteData(
                table: "document_types",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "lease_statuses",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "agreement_document_id",
                table: "leases");

            migrationBuilder.DropColumn(
                name: "signed_agreement_document_id",
                table: "leases");

            // Temporary renames to free up unique names on Down
            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "TEMP_APPROVED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "TEMP_REJECTED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "TEMP_EXPIRED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "TEMP_CANCELLED");

            // Restore original names
            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "UNDER_REVIEW");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "APPROVED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "REJECTED");

            migrationBuilder.UpdateData(
                table: "proposal_statuses",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "CANCELLED");

            migrationBuilder.InsertData(
                table: "proposal_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 7, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "EXPIRED", null },
                    { 8, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "NEGOTIATED", null }
                });
        }
    }
}
