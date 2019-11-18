using Microsoft.EntityFrameworkCore.Migrations;

namespace TBIProject.Data.Migrations
{
    public partial class AddedHasChangedPasswordPropertyToUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasChangedPassword",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasChangedPassword",
                table: "AspNetUsers");
        }
    }
}
