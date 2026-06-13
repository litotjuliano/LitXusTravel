using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitXusTravel.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantDefaultCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "MYR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "Tenants");
        }
    }
}
