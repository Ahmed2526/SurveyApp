using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultAdminAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111", "fcc14079-6487-4984-9580-2588e3297483", false, false, "Admin", "ADMIN" },
                    { "019a0ee5-cea2-7ffa-9ebe-27d9dad81e83", "bec0a637-a754-4d26-b215-cb584b76cf06", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "019a0ee5-cea2-7ffa-9ebe-27d88ee5d125", 0, "1b149a6b-fda3-445f-9efc-9828f4b3f6bf", "admin@SurveyApp.com", true, false, null, "ADMIN@SURVEYAPP.COM", "ADMIN", "AQAAAAIAAYagAAAAEIEXWHyGWXXcDowlFIMDok1qGSF7lfYun9bnolKMYM7TndQ99d09pCCnEZO55KM6iw==", "01512345678", true, "30e479f7-fcf6-4a40-a456-c4a3d595d420", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "create_survey", "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111" },
                    { 2, "Permissions", "edit_survey", "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111" },
                    { 3, "Permissions", "delete_survey", "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111" },
                    { 4, "Permissions", "view_survey", "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111", "019a0ee5-cea2-7ffa-9ebe-27d88ee5d125" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019a0ee5-cea2-7ffa-9ebe-27d9dad81e83");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111", "019a0ee5-cea2-7ffa-9ebe-27d88ee5d125" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019a0ee5-cea2-7ffa-9ebe-27d88ee5d125");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetRoles");
        }
    }
}
