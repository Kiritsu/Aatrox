using Microsoft.EntityFrameworkCore.Migrations;

namespace Aatrox.Data.Migrations
{
    public partial class blacklist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Premium",
                table: "user_entity",
                newName: "premium");

            migrationBuilder.AddColumn<bool>(
                name: "blacklisted",
                table: "user_entity",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blacklisted",
                table: "user_entity");

            migrationBuilder.RenameColumn(
                name: "premium",
                table: "user_entity",
                newName: "Premium");
        }
    }
}
