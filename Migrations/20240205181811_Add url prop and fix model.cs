using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuyAndSell.Migrations
{
    public partial class Addurlpropandfixmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photos",
                table: "Ads");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Ads",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Ads");

            migrationBuilder.AddColumn<List<string>>(
                name: "Photos",
                table: "Ads",
                type: "text[]",
                nullable: false);
        }
    }
}
