using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerificationAndActiveStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "active_status_id",
                table: "users",
                type: "integer",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "verification_status_id",
                table: "users",
                type: "integer",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "user_active_statuses",
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
                    table.PrimaryKey("user_active_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_verification_statuses",
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
                    table.PrimaryKey("user_verification_statuses_pkey", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "user_active_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Active", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Inactive", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Suspended", null }
                });

            migrationBuilder.InsertData(
                table: "user_verification_statuses",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Unverified", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Pending", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Verified", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Rejected", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_active_status_id",
                table: "users",
                column: "active_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_verification_status_id",
                table: "users",
                column: "verification_status_id");

            migrationBuilder.CreateIndex(
                name: "user_active_statuses_name_key",
                table: "user_active_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_verification_statuses_name_key",
                table: "user_verification_statuses",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "users_active_status_id_fkey",
                table: "users",
                column: "active_status_id",
                principalTable: "user_active_statuses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "users_verification_status_id_fkey",
                table: "users",
                column: "verification_status_id",
                principalTable: "user_verification_statuses",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "users_active_status_id_fkey",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "users_verification_status_id_fkey",
                table: "users");

            migrationBuilder.DropTable(
                name: "user_active_statuses");

            migrationBuilder.DropTable(
                name: "user_verification_statuses");

            migrationBuilder.DropIndex(
                name: "IX_users_active_status_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_verification_status_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "active_status_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "verification_status_id",
                table: "users");
        }
    }
}
