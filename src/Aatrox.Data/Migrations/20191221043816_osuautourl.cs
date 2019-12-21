using Microsoft.EntityFrameworkCore.Migrations;

namespace Aatrox.Data.Migrations
{
    public partial class osuautourl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "auto_resolve_osu_url",
                table: "guild_entity",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auto_resolve_osu_url",
                table: "guild_entity");
        }
    }
}
