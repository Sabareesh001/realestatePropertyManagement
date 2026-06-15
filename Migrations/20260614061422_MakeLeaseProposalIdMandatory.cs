using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class MakeLeaseProposalIdMandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "leases_proposal_id_fkey",
                table: "leases");

            migrationBuilder.AlterColumn<Guid>(
                name: "proposal_id",
                table: "leases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "leases_proposal_id_fkey",
                table: "leases",
                column: "proposal_id",
                principalTable: "lease_proposals",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "leases_proposal_id_fkey",
                table: "leases");

            migrationBuilder.AlterColumn<Guid>(
                name: "proposal_id",
                table: "leases",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "leases_proposal_id_fkey",
                table: "leases",
                column: "proposal_id",
                principalTable: "lease_proposals",
                principalColumn: "id");
        }
    }
}
