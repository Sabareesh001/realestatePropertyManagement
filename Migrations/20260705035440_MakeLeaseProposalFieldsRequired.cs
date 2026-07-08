using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class MakeLeaseProposalFieldsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lease_proposals_property_id_fkey",
                table: "lease_proposals");

            migrationBuilder.DropForeignKey(
                name: "lease_proposals_status_id_fkey",
                table: "lease_proposals");

            migrationBuilder.DropForeignKey(
                name: "lease_proposals_tenant_id_fkey",
                table: "lease_proposals");

            migrationBuilder.AlterColumn<decimal>(
                name: "upfront_payment",
                table: "lease_proposals",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "tenant_id",
                table: "lease_proposals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "lease_proposals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "start_date",
                table: "lease_proposals",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "security_deposit",
                table: "lease_proposals",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "property_id",
                table: "lease_proposals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "monthly_rent",
                table: "lease_proposals",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "end_date",
                table: "lease_proposals",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "lease_proposals",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "lease_proposals_property_id_fkey",
                table: "lease_proposals",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "lease_proposals_status_id_fkey",
                table: "lease_proposals",
                column: "status_id",
                principalTable: "proposal_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "lease_proposals_tenant_id_fkey",
                table: "lease_proposals",
                column: "tenant_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lease_proposals_property_id_fkey",
                table: "lease_proposals");

            migrationBuilder.DropForeignKey(
                name: "lease_proposals_status_id_fkey",
                table: "lease_proposals");

            migrationBuilder.DropForeignKey(
                name: "lease_proposals_tenant_id_fkey",
                table: "lease_proposals");

            migrationBuilder.AlterColumn<decimal>(
                name: "upfront_payment",
                table: "lease_proposals",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "tenant_id",
                table: "lease_proposals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "lease_proposals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "start_date",
                table: "lease_proposals",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<decimal>(
                name: "security_deposit",
                table: "lease_proposals",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "property_id",
                table: "lease_proposals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "monthly_rent",
                table: "lease_proposals",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "end_date",
                table: "lease_proposals",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "lease_proposals",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddForeignKey(
                name: "lease_proposals_property_id_fkey",
                table: "lease_proposals",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "lease_proposals_status_id_fkey",
                table: "lease_proposals",
                column: "status_id",
                principalTable: "proposal_statuses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "lease_proposals_tenant_id_fkey",
                table: "lease_proposals",
                column: "tenant_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
