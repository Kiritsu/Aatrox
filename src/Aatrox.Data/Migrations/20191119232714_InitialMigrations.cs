using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aatrox.Data.Migrations
{
    public partial class InitialMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guild_entity",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    prefixes = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_entity", x => x.snowflake_id);
                });

            migrationBuilder.CreateTable(
                name: "user_entity",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    language = table.Column<int>(nullable: false),
                    Premium = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_entity", x => x.snowflake_id);
                });

            migrationBuilder.CreateTable(
                name: "league_user_entity",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    username = table.Column<string>(nullable: true),
                    region = table.Column<string>(nullable: true),
                    channels = table.Column<List<long>>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "osu_user_entity",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    username = table.Column<string>(nullable: true),
                    channels = table.Column<List<long>>(nullable: true),
                    send_recent_score = table.Column<bool>(nullable: false),
                    send_new_best_score = table.Column<bool>(nullable: false),
                    pp_min = table.Column<int>(nullable: false),
                    country_rank_min = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_osu_user_entity", x => x.snowflake_id);
                    table.ForeignKey(
                        name: "fkey_osu_user_entity_user_id",
                        column: x => x.snowflake_id,
                        principalTable: "user_entity",
                        principalColumn: "snowflake_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_entity");

            migrationBuilder.DropTable(
                name: "league_user_entity");

            migrationBuilder.DropTable(
                name: "osu_user_entity");

            migrationBuilder.DropTable(
                name: "user_entity");
        }
    }
}
