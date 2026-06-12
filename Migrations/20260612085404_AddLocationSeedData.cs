using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "countries",
                columns: new[] { "id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[] { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "India", null });

            migrationBuilder.InsertData(
                table: "states",
                columns: new[] { "id", "country_id", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[] { 1, 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tamil Nadu", null });

            migrationBuilder.InsertData(
                table: "districts",
                columns: new[] { "id", "created_at", "deleted_at", "name", "state_id", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ariyalur", 1, null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Chengalpattu", 1, null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Chennai", 1, null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Coimbatore", 1, null },
                    { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Cuddalore", 1, null },
                    { 6, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Dharmapuri", 1, null },
                    { 7, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Dindigul", 1, null },
                    { 8, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Erode", 1, null },
                    { 9, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Kallakurichi", 1, null },
                    { 10, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Kanchipuram", 1, null },
                    { 11, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Kanyakumari", 1, null },
                    { 12, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Karur", 1, null },
                    { 13, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Krishnagiri", 1, null },
                    { 14, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Madurai", 1, null },
                    { 15, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mayiladuthurai", 1, null },
                    { 16, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Nagapattinam", 1, null },
                    { 17, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Namakkal", 1, null },
                    { 18, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Nilgiris", 1, null },
                    { 19, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Perambalur", 1, null },
                    { 20, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Pudukkottai", 1, null },
                    { 21, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ramanathapuram", 1, null },
                    { 22, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ranipet", 1, null },
                    { 23, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Salem", 1, null },
                    { 24, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Sivaganga", 1, null },
                    { 25, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tenkasi", 1, null },
                    { 26, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Thanjavur", 1, null },
                    { 27, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Theni", 1, null },
                    { 28, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Thoothukudi", 1, null },
                    { 29, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tiruchirappalli", 1, null },
                    { 30, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tirunelveli", 1, null },
                    { 31, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tirupattur", 1, null },
                    { 32, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tiruppur", 1, null },
                    { 33, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tiruvallur", 1, null },
                    { 34, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tiruvannamalai", 1, null },
                    { 35, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tiruvarur", 1, null },
                    { 36, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Vellore", 1, null },
                    { 37, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Viluppuram", 1, null },
                    { 38, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Virudhunagar", 1, null }
                });

            migrationBuilder.InsertData(
                table: "cities",
                columns: new[] { "id", "created_at", "deleted_at", "district_id", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "Ariyalur", null },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, "Chengalpattu", null },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, "Chennai", null },
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4, "Coimbatore", null },
                    { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 5, "Cuddalore", null },
                    { 6, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 6, "Dharmapuri", null },
                    { 7, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 7, "Dindigul", null },
                    { 8, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 8, "Erode", null },
                    { 9, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 9, "Kallakurichi", null },
                    { 10, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 10, "Kanchipuram", null },
                    { 11, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 11, "Nagercoil", null },
                    { 12, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 12, "Karur", null },
                    { 13, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 13, "Krishnagiri", null },
                    { 14, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 14, "Madurai", null },
                    { 15, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 15, "Mayiladuthurai", null },
                    { 16, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 16, "Nagapattinam", null },
                    { 17, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 17, "Namakkal", null },
                    { 18, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 18, "Udhagamandalam", null },
                    { 19, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 19, "Perambalur", null },
                    { 20, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 20, "Pudukkottai", null },
                    { 21, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 21, "Ramanathapuram", null },
                    { 22, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 22, "Ranipet", null },
                    { 23, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 23, "Salem", null },
                    { 24, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 24, "Sivaganga", null },
                    { 25, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 25, "Tenkasi", null },
                    { 26, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 26, "Thanjavur", null },
                    { 27, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 27, "Theni", null },
                    { 28, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 28, "Thoothukudi", null },
                    { 29, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 29, "Tiruchirappalli", null },
                    { 30, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 30, "Tirunelveli", null },
                    { 31, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 31, "Tirupattur", null },
                    { 32, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 32, "Tiruppur", null },
                    { 33, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 33, "Tiruvallur", null },
                    { 34, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 34, "Tiruvannamalai", null },
                    { 35, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 35, "Tiruvarur", null },
                    { 36, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 36, "Vellore", null },
                    { 37, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 37, "Viluppuram", null },
                    { 38, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 38, "Virudhunagar", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "districts",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "states",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
