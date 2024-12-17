using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class updatetabletiktokcampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add a temporary boolean column
            migrationBuilder.AddColumn<bool>(
                name: "SearchFeedTemp",
                table: "CampaignContents",
                nullable: false,
                defaultValue: false);

            // Convert existing string data ('true', 'false') into boolean
            migrationBuilder.Sql(
                "UPDATE CampaignContents SET SearchFeedTemp = CASE WHEN SearchFeed = 'true' THEN 1 ELSE 0 END");

            // Drop the old string column
            migrationBuilder.DropColumn(
                name: "SearchFeed",
                table: "CampaignContents");

            // Rename the temporary column to the original name
            migrationBuilder.RenameColumn(
                name: "SearchFeedTemp",
                table: "CampaignContents",
                newName: "SearchFeed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add the original string column back
            migrationBuilder.AddColumn<string>(
                name: "SearchFeed",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            // Convert boolean back into string data ('true', 'false')
            migrationBuilder.Sql(
                "UPDATE CampaignContents SET SearchFeed = CASE WHEN SearchFeedTemp = 1 THEN 'true' ELSE 'false' END");

            // Drop the temporary boolean column
            migrationBuilder.DropColumn(
                name: "SearchFeedTemp",
                table: "CampaignContents");
        }

    }
}
