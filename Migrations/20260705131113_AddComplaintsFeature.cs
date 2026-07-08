using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintsFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "complaints");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "complaints",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "attachment_url",
                table: "complaints",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                table: "complaints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "lease_id",
                table: "complaints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "complaints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subject",
                table: "complaints",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "complaint_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complaint_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("complaint_comments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "complaint_comments_author_id_fkey",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "complaint_comments_complaint_id_fkey",
                        column: x => x.complaint_id,
                        principalTable: "complaints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_complaints_created_by",
                table: "complaints",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_lease_id",
                table: "complaints",
                column: "lease_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_owner_id",
                table: "complaints",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaint_comments_author_id",
                table: "complaint_comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaint_comments_complaint_id",
                table: "complaint_comments",
                column: "complaint_id");

            migrationBuilder.AddForeignKey(
                name: "complaints_created_by_fkey",
                table: "complaints",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "complaints_lease_id_fkey",
                table: "complaints",
                column: "lease_id",
                principalTable: "leases",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "complaints_owner_id_fkey",
                table: "complaints",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id");

            // Seed complaint categories (complaint_types)
            migrationBuilder.InsertData(
                table: "complaint_types",
                columns: new[] { "id", "name", "created_at", "updated_at", "deleted_at" },
                values: new object[,]
                {
                    { 1, "Maintenance",       new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, "Plumbing",          new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 3, "Electrical",        new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 4, "Appliance",         new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 5, "Noise/Neighbours",  new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 6, "Security/Safety",   new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 7, "Billing/Payment",   new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 8, "Other",             new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            // Seed complaint priorities
            migrationBuilder.InsertData(
                table: "complaint_priorities",
                columns: new[] { "id", "name", "created_at", "updated_at", "deleted_at" },
                values: new object[,]
                {
                    { 1, "Low",    new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, "Medium", new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 3, "High",   new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 4, "Urgent", new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            // Seed complaint statuses
            migrationBuilder.InsertData(
                table: "complaint_statuses",
                columns: new[] { "id", "name", "created_at", "updated_at", "deleted_at" },
                values: new object[,]
                {
                    { 1, "Open",       new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, "InProgress", new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 3, "Resolved",   new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 4, "Closed",     new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 5, "Cancelled",  new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "complaints_created_by_fkey",
                table: "complaints");

            migrationBuilder.DropForeignKey(
                name: "complaints_lease_id_fkey",
                table: "complaints");

            migrationBuilder.DropForeignKey(
                name: "complaints_owner_id_fkey",
                table: "complaints");

            migrationBuilder.DropTable(
                name: "complaint_comments");

            migrationBuilder.DropIndex(
                name: "IX_complaints_created_by",
                table: "complaints");

            migrationBuilder.DropIndex(
                name: "IX_complaints_lease_id",
                table: "complaints");

            migrationBuilder.DropIndex(
                name: "IX_complaints_owner_id",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "attachment_url",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "lease_id",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "subject",
                table: "complaints");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "complaints",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "complaints",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.DeleteData(table: "complaint_types", keyColumn: "id", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            migrationBuilder.DeleteData(table: "complaint_priorities", keyColumn: "id", keyValues: new object[] { 1, 2, 3, 4 });
            migrationBuilder.DeleteData(table: "complaint_statuses", keyColumn: "id", keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
