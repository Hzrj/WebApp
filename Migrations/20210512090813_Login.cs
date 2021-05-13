using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class Login : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Login",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Now = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Online = table.Column<bool>(type: "bit", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    Enable = table.Column<bool>(type: "bit", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RePassword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Login");
        }
    }
}
