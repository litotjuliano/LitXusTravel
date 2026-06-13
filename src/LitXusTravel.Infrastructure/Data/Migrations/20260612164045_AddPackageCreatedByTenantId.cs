using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitXusTravel.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageCreatedByTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByTenantId",
                table: "Packages",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByTenantId",
                table: "Packages");
        }
    }
}
