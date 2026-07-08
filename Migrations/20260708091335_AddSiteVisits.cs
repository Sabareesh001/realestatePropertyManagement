using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "specific_visit_days",
                table: "properties",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "visit_end_time",
                table: "properties",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "visit_preferences",
                table: "properties",
                type: "character varying(50)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "visit_start_time",
                table: "properties",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "site_visit_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("site_visit_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site_visits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    property_id = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    remarks = table.Column<string>(type: "character varying(500)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("site_visits_pkey", x => x.id);
                    table.ForeignKey(
                        name: "site_visits_owner_id_fkey",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "site_visits_property_id_fkey",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "site_visits_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "site_visit_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "site_visits_tenant_id_fkey",
                        column: x => x.tenant_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "site_visit_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Pending", null },
                    { 2, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Approved", null },
                    { 3, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Cancelled", null }
                });

            migrationBuilder.CreateIndex(
                name: "site_visit_statuses_name_key",
                table: "site_visit_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_visits_owner_id",
                table: "site_visits",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_visits_property_id",
                table: "site_visits",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_visits_status_id",
                table: "site_visits",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_visits_tenant_id",
                table: "site_visits",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_visits");

            migrationBuilder.DropTable(
                name: "site_visit_statuses");

            migrationBuilder.DropColumn(
                name: "specific_visit_days",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "visit_end_time",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "visit_preferences",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "visit_start_time",
                table: "properties");
        }
    }
}
