using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerificationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    remarks = table.Column<string>(type: "text", nullable: true),
                    verified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_verifications_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_verifications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "user_verifications_verified_by_fkey",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_verification_documents",
                columns: table => new
                {
                    user_verification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_verification_documents_pkey", x => new { x.user_verification_id, x.document_id });
                    table.ForeignKey(
                        name: "user_verification_documents_document_id_fkey",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "user_verification_documents_user_verification_id_fkey",
                        column: x => x.user_verification_id,
                        principalTable: "user_verifications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_verification_documents_document_id",
                table: "user_verification_documents",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_verifications_user_id",
                table: "user_verifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_verifications_verified_by",
                table: "user_verifications",
                column: "verified_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_verification_documents");

            migrationBuilder.DropTable(
                name: "user_verifications");
        }
    }
}
