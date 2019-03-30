using Microsoft.EntityFrameworkCore.Migrations;

namespace Element.Data.Migrations
{
    public partial class add_user_entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_entity",
                columns: table => new
                {
                    user_id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_entity", x => x.user_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_entity");
        }
    }
}
