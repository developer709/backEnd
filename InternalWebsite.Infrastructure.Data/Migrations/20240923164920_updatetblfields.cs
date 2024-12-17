using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class updatetblfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdFormat",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageName",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebUrl",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "CampaignBudgets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "CampaignBudgets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasEndDate",
                table: "CampaignBudgets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "CampaignBudgets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "CampaignBudgets",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdFormat",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "Caption",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "PageName",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "WebUrl",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "CampaignBudgets");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "CampaignBudgets");

            migrationBuilder.DropColumn(
                name: "HasEndDate",
                table: "CampaignBudgets");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "CampaignBudgets");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "CampaignBudgets");
        }
    }
}
