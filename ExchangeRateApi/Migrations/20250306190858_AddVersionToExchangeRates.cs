using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeRateApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionToExchangeRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "ExchangeRates",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "ExchangeRates");
        }
    }
}
