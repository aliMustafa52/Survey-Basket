using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasketV3.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0190eec2-db24-71ca-86f1-51dda625b4af", "0190eec4-a07f-78bb-9e72-7c165a3cb1c8", false, false, "Admin", "ADMIN" },
                    { "0190eec3-5d1f-734d-a8fe-2259e907f8ed", "0190eec4-d9c6-7404-93bc-56147bc59272", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0190eeab-66d6-7bae-a7de-777d646034bb", 0, "0190eeae-68aa-7816-8302-6637672f6c19", "admin@servey-basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SERVEY-BASKET.COM", "ADMIN@SERVEY-BASKET.COM", "AQAAAAIAAYagAAAAED5sG0iizD3nU40lDXu/uj9DbidkmJ1BnXihKI8Kmvu3oxGdh9HPLwPUexv4qmMYGA==", null, false, "0190EEAEB76A78E8AE5FE351F56B0065", false, "admin@servey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "Polls:Get", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 2, "permissions", "Polls:Add", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 3, "permissions", "Polls:Update", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 4, "permissions", "Polls:Delete", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 5, "permissions", "Questions:Get", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 6, "permissions", "Questions:Add", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 7, "permissions", "Questions:Update", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 8, "permissions", "Results:Get", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 9, "permissions", "Users:Get", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 10, "permissions", "Users:Add", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 11, "permissions", "Users:Update", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 12, "permissions", "Roles:Get", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 13, "permissions", "Roles:Add", "0190eec2-db24-71ca-86f1-51dda625b4af" },
                    { 14, "permissions", "Roles:Update", "0190eec2-db24-71ca-86f1-51dda625b4af" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0190eec2-db24-71ca-86f1-51dda625b4af", "0190eeab-66d6-7bae-a7de-777d646034bb" });
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
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0190eec3-5d1f-734d-a8fe-2259e907f8ed");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0190eec2-db24-71ca-86f1-51dda625b4af", "0190eeab-66d6-7bae-a7de-777d646034bb" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0190eec2-db24-71ca-86f1-51dda625b4af");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0190eeab-66d6-7bae-a7de-777d646034bb");
        }
    }
}
