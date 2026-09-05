using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NerjaLogisticsERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeProfilePicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureContentType",
                table: "Employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureStorageKey",
                table: "Employees",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureContentType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ProfilePictureStorageKey",
                table: "Employees");
        }
    }
}
