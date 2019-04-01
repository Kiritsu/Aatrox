using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Element.Data.Migrations
{
    public partial class add_guild_entity : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_entity");
        }
    }
}
