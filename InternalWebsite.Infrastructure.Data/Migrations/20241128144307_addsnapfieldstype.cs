using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class addsnapfieldstype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdDisclaimer",
                table: "CampaignContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AdFavouriting",
                table: "CampaignContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BtnColor",
                table: "CampaignContents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdDisclaimer",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "AdFavouriting",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "BtnColor",
                table: "CampaignContents");
        }
    }
}
