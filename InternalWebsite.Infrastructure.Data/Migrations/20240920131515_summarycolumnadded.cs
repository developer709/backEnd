using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalWebsite.Infrastructure.Data.Migrations
{
    public partial class summarycolumnadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Marketings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Marketings");
        }
    }
}
