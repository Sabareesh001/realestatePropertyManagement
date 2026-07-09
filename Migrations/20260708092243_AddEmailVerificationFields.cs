using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email_verification_hash",
                table: "users",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "email_verification_hash_expires_at",
                table: "users",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "email_verified",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email_verification_hash",
                table: "users");

            migrationBuilder.DropColumn(
                name: "email_verification_hash_expires_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "email_verified",
                table: "users");
        }
    }
}
