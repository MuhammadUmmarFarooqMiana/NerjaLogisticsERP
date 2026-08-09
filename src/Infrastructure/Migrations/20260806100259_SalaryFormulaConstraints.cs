using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NerjaLogisticsERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalaryFormulaConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_SalaryFormulas_EffectiveTo_After_EffectiveFrom",
                table: "SalaryFormulas",
                sql: "\"EffectiveTo\" IS NULL OR \"EffectiveTo\" > \"EffectiveFrom\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SalaryFormulas_EffectiveTo_After_EffectiveFrom",
                table: "SalaryFormulas");
        }
    }
}
