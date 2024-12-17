using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class updatetabletiktok : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiktokIdentityContentCollections_AspNetUsers_UserId",
                table: "TiktokIdentityContentCollections");

            migrationBuilder.DropIndex(
                name: "IX_TiktokIdentityContentCollections_UserId",
                table: "TiktokIdentityContentCollections");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TiktokIdentityContentCollections");

            migrationBuilder.CreateIndex(
                name: "IX_TiktokIdentityContentCollections_CampaignId",
                table: "TiktokIdentityContentCollections",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_TiktokIdentityContentCollections_Campaigns_CampaignId",
                table: "TiktokIdentityContentCollections",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiktokIdentityContentCollections_Campaigns_CampaignId",
                table: "TiktokIdentityContentCollections");

            migrationBuilder.DropIndex(
                name: "IX_TiktokIdentityContentCollections_CampaignId",
                table: "TiktokIdentityContentCollections");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TiktokIdentityContentCollections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TiktokIdentityContentCollections_UserId",
                table: "TiktokIdentityContentCollections",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TiktokIdentityContentCollections_AspNetUsers_UserId",
                table: "TiktokIdentityContentCollections",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
