using Microsoft.EntityFrameworkCore.Migrations;

namespace TBIProject.Data.Migrations
{
    public partial class operatorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperatorId",
                table: "Applications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OperatorId",
                table: "Applications",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AspNetUsers_OperatorId",
                table: "Applications",
                column: "OperatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AspNetUsers_OperatorId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_OperatorId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                table: "Applications");
        }
    }
}
