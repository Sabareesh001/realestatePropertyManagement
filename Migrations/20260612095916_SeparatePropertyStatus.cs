using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class SeparatePropertyStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "properties_status_id_fkey",
                table: "properties");

            migrationBuilder.DropTable(
                name: "property_statuses");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "properties",
                newName: "verification_status_id");

            migrationBuilder.RenameIndex(
                name: "IX_properties_status_id",
                table: "properties",
                newName: "IX_properties_verification_status_id");

            migrationBuilder.AddColumn<int>(
                name: "availability_status_id",
                table: "properties",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "property_availability_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("property_availability_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "property_verification_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("property_verification_statuses_pkey", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "property_availability_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Available", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Occupied", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Unavailable", null }
                });

            migrationBuilder.InsertData(
                table: "property_verification_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Draft", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Submitted", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Verified", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Rejected", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_properties_availability_status_id",
                table: "properties",
                column: "availability_status_id");

            migrationBuilder.CreateIndex(
                name: "property_availability_statuses_name_key",
                table: "property_availability_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "property_verification_statuses_name_key",
                table: "property_verification_statuses",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "properties_availability_status_id_fkey",
                table: "properties",
                column: "availability_status_id",
                principalTable: "property_availability_statuses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "properties_verification_status_id_fkey",
                table: "properties",
                column: "verification_status_id",
                principalTable: "property_verification_statuses",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "properties_availability_status_id_fkey",
                table: "properties");

            migrationBuilder.DropForeignKey(
                name: "properties_verification_status_id_fkey",
                table: "properties");

            migrationBuilder.DropTable(
                name: "property_availability_statuses");

            migrationBuilder.DropTable(
                name: "property_verification_statuses");

            migrationBuilder.DropIndex(
                name: "IX_properties_availability_status_id",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "availability_status_id",
                table: "properties");

            migrationBuilder.RenameColumn(
                name: "verification_status_id",
                table: "properties",
                newName: "status_id");

            migrationBuilder.RenameIndex(
                name: "IX_properties_verification_status_id",
                table: "properties",
                newName: "IX_properties_status_id");

            migrationBuilder.CreateTable(
                name: "property_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("property_statuses_pkey", x => x.id);
                });

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

            migrationBuilder.CreateIndex(
                name: "property_statuses_name_key",
                table: "property_statuses",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "properties_status_id_fkey",
                table: "properties",
                column: "status_id",
                principalTable: "property_statuses",
                principalColumn: "id");
        }
    }
}
