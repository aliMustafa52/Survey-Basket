using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasketV3.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDisabledColumnToUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0190eeab-66d6-7bae-a7de-777d646034bb",
                columns: new[] { "IsDisabled", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAEMz5Rb234N1xoGLSrDCZGMzm04MFb8f6c38fdHunyWyt9f+NwUbtbadZL90jgPOhig==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0190eeab-66d6-7bae-a7de-777d646034bb",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAED5sG0iizD3nU40lDXu/uj9DbidkmJ1BnXihKI8Kmvu3oxGdh9HPLwPUexv4qmMYGA==");
        }
    }
}
