using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLeaseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lease_proposals_lease_type_id_fkey",
                table: "lease_proposals");

            migrationBuilder.DropForeignKey(
                name: "leases_lease_type_id_fkey",
                table: "leases");

            migrationBuilder.DropTable(
                name: "lease_types");

            migrationBuilder.DropIndex(
                name: "IX_leases_lease_type_id",
                table: "leases");

            migrationBuilder.DropIndex(
                name: "IX_lease_proposals_lease_type_id",
                table: "lease_proposals");

            migrationBuilder.DropColumn(
                name: "lease_type_id",
                table: "leases");

            migrationBuilder.DropColumn(
                name: "lease_type_id",
                table: "lease_proposals");

            migrationBuilder.CreateTable(
                name: "bank_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    account_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    account_holder_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ifsc_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bank_accounts_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_bank_accounts",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_account_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_bank_accounts_pkey", x => new { x.user_id, x.bank_account_id });
                    table.ForeignKey(
                        name: "user_bank_accounts_bank_account_id_fkey",
                        column: x => x.bank_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "user_bank_accounts_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_bank_accounts_bank_account_id",
                table: "user_bank_accounts",
                column: "bank_account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_bank_accounts");

            migrationBuilder.DropTable(
                name: "bank_accounts");

            migrationBuilder.AddColumn<int>(
                name: "lease_type_id",
                table: "leases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "lease_type_id",
                table: "lease_proposals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "lease_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lease_types_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_leases_lease_type_id",
                table: "leases",
                column: "lease_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_lease_proposals_lease_type_id",
                table: "lease_proposals",
                column: "lease_type_id");

            migrationBuilder.CreateIndex(
                name: "lease_types_name_key",
                table: "lease_types",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "lease_proposals_lease_type_id_fkey",
                table: "lease_proposals",
                column: "lease_type_id",
                principalTable: "lease_types",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "leases_lease_type_id_fkey",
                table: "leases",
                column: "lease_type_id",
                principalTable: "lease_types",
                principalColumn: "id");
        }
    }
}
