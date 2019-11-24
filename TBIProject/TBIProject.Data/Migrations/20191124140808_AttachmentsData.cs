using Microsoft.EntityFrameworkCore.Migrations;

namespace TBIProject.Data.Migrations
{
    public partial class AttachmentsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttachmentSize",
                table: "Applications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttachmentsCount",
                table: "Applications",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentSize",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AttachmentsCount",
                table: "Applications");
        }
    }
}
