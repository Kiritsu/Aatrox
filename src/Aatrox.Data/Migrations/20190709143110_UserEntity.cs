using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aatrox.Data.Migrations
{
    public partial class UserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_entity",
                columns: table => new
                {
                    snowflake_id = table.Column<decimal>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    language = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_entity", x => x.snowflake_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_entity");
        }
    }
}
