using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class addedcustomages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndAge",
                table: "CampaignAudiences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartAge",
                table: "CampaignAudiences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndAge",
                table: "CampaignAudiences");

            migrationBuilder.DropColumn(
                name: "StartAge",
                table: "CampaignAudiences");
        }
    }
}
