using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class deletesnapfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdDisclaimer",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdFavouriting",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BtnColor",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
