using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aatrox.Data.Migrations
{
    public partial class LeagueUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "league_user_entity",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    username = table.Column<string>(nullable: true),
                    region = table.Column<string>(nullable: true),
                    channels = table.Column<List<ulong>>(nullable: true),
                    send_current_game_info = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league_user_entity", x => x.snowflake_id);
                    table.ForeignKey(
                        name: "fkey_league_user_entity_user_id",
                        column: x => x.snowflake_id,
                        principalTable: "user_entity",
                        principalColumn: "snowflake_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "league_user_entity");
        }
    }
}
