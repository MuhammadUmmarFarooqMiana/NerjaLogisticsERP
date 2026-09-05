using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NerjaLogisticsERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyDocumentFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FolderId",
                table: "CompanyDocuments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyDocumentFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ParentFolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDocumentFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyDocumentFolders_CompanyDocumentFolders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "CompanyDocumentFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDocuments_FolderId",
                table: "CompanyDocuments",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDocumentFolders_ParentFolderId_Name",
                table: "CompanyDocumentFolders",
                columns: new[] { "ParentFolderId", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDocuments_CompanyDocumentFolders_FolderId",
                table: "CompanyDocuments",
                column: "FolderId",
                principalTable: "CompanyDocumentFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDocuments_CompanyDocumentFolders_FolderId",
                table: "CompanyDocuments");

            migrationBuilder.DropTable(
                name: "CompanyDocumentFolders");

            migrationBuilder.DropIndex(
                name: "IX_CompanyDocuments_FolderId",
                table: "CompanyDocuments");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "CompanyDocuments");
        }
    }
}
