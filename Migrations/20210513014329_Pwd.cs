using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class Pwd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Login",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RePassword",
                table: "Login",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Login");

            migrationBuilder.DropColumn(
                name: "RePassword",
                table: "Login");
        }
    }
}
