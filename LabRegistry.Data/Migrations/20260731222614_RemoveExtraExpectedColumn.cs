using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabRegistry.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraExpectedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_expectedReturnDate",
                table: "Loan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "_expectedReturnDate",
                table: "Loan",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
