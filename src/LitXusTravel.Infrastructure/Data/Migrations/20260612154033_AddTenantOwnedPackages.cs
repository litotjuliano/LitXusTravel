using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitXusTravel.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantOwnedPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantPackages_TenantId_MasterPackageId",
                table: "TenantPackages");

            migrationBuilder.AlterColumn<Guid>(
                name: "MasterPackageId",
                table: "TenantPackages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsOwnedPackage",
                table: "TenantPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PackageOverrides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "PackageOverrides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationDays",
                table: "PackageOverrides",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "PackageOverrides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantPackages_TenantId_MasterPackageId",
                table: "TenantPackages",
                columns: new[] { "TenantId", "MasterPackageId" },
                unique: true,
                filter: "[MasterPackageId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantPackages_TenantId_MasterPackageId",
                table: "TenantPackages");

            migrationBuilder.DropColumn(
                name: "IsOwnedPackage",
                table: "TenantPackages");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "PackageOverrides");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "PackageOverrides");

            migrationBuilder.DropColumn(
                name: "DurationDays",
                table: "PackageOverrides");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "PackageOverrides");

            migrationBuilder.AlterColumn<Guid>(
                name: "MasterPackageId",
                table: "TenantPackages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantPackages_TenantId_MasterPackageId",
                table: "TenantPackages",
                columns: new[] { "TenantId", "MasterPackageId" },
                unique: true);
        }
    }
}
