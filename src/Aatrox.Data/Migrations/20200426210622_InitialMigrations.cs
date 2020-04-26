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
                name: "guilds",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    prefixes = table.Column<List<string>>(nullable: true),
                    resolve_osu_urls = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guilds", x => x.snowflake_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    language = table.Column<int>(nullable: false),
                    premium = table.Column<bool>(nullable: false),
                    blacklisted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.snowflake_id);
                });

            migrationBuilder.CreateTable(
                name: "league_users",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    username = table.Column<string>(nullable: true),
                    region = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league_users", x => x.snowflake_id);
                    table.ForeignKey(
                        name: "fkey_league_user_entity_user_id",
                        column: x => x.snowflake_id,
                        principalTable: "users",
                        principalColumn: "snowflake_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "osu_users",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_osu_users", x => x.snowflake_id);
                    table.ForeignKey(
                        name: "fkey_osu_user_entity_user_id",
                        column: x => x.snowflake_id,
                        principalTable: "users",
                        principalColumn: "snowflake_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guilds");

            migrationBuilder.DropTable(
                name: "league_users");

            migrationBuilder.DropTable(
                name: "osu_users");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
