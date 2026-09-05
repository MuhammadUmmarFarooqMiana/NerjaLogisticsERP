using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NerjaLogisticsERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DailyOrderReviewWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewNote",
                table: "DailyOrders",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewedAt",
                table: "DailyOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedBy",
                table: "DailyOrders",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewNote",
                table: "DailyOrders");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "DailyOrders");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "DailyOrders");
        }
    }
}
