using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class updateThumbnailCollectionnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ThumbnailCollection",
                table: "CampaignContents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ThumbnailCollection",
                table: "CampaignContents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
