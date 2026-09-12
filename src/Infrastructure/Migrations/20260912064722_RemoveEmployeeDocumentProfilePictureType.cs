using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NerjaLogisticsERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmployeeDocumentProfilePictureType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EmployeeDocumentType.ProfilePicture no longer exists in C#, but Type is stored as
            // its enum name (HasConversion<string>()) — any pre-existing row with Type =
            // 'ProfilePicture' would throw when EF tries to parse it back. Before removing those
            // rows, carry their file reference onto Employee's own dedicated field, but only
            // where that field is still empty — an employee who already has a newer picture set
            // through the dedicated upload keeps it, rather than being reverted to an older one.
            migrationBuilder.Sql(
                """
                UPDATE "Employees" e
                SET "ProfilePictureStorageKey" = ed."StorageKey",
                    "ProfilePictureContentType" = ed."ContentType"
                FROM "EmployeeDocuments" ed
                WHERE ed."EmployeeId" = e."Id"
                  AND ed."Type" = 'ProfilePicture'
                  AND e."ProfilePictureStorageKey" IS NULL;
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM "EmployeeDocuments" WHERE "Type" = 'ProfilePicture';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Not reversible: the EmployeeDocument rows deleted in Up() are gone, and there's no
            // record of which employees' ProfilePictureStorageKey came from that backfill versus
            // an unrelated later upload, so there's nothing sound to restore.
        }
    }
}
