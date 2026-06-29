using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tijori.Infrastructure.Data.Migrations
{
    public partial class RemovePhoneVerification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhoneVerified",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneVerified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
