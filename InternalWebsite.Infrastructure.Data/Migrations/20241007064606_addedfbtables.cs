using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class addedfbtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignContentCollections_CampaignContents_CampaignContentId",
                table: "CampaignContentCollections");

            migrationBuilder.DropTable(
                name: "FaceboobCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_CampaignContentCollections_CampaignContentId",
                table: "CampaignContentCollections");

            migrationBuilder.DropColumn(
                name: "CampaignContentId",
                table: "CampaignContentCollections");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "CampaignContentCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FacebookCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FbCampaignId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdSetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdCreativeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookCampaigns_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookCampaigns_UserId",
                table: "FacebookCampaigns",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookCampaigns");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "CampaignContentCollections");

            migrationBuilder.AddColumn<Guid>(
                name: "CampaignContentId",
                table: "CampaignContentCollections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FaceboobCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdCreativeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdSetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FbCampaignId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceboobCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceboobCampaigns_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContentCollections_CampaignContentId",
                table: "CampaignContentCollections",
                column: "CampaignContentId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceboobCampaigns_UserId",
                table: "FaceboobCampaigns",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignContentCollections_CampaignContents_CampaignContentId",
                table: "CampaignContentCollections",
                column: "CampaignContentId",
                principalTable: "CampaignContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
