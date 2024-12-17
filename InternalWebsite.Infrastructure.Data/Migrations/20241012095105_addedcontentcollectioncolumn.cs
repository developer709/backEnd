using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class addedcontentcollectioncolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "CampaignContentCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Headline",
                table: "CampaignContentCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CampaignContentCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "CampaignContentCollections",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "CampaignContentCollections");

            migrationBuilder.DropColumn(
                name: "Headline",
                table: "CampaignContentCollections");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CampaignContentCollections");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "CampaignContentCollections");
        }
    }
}
