using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Simbir.Health.AccountAPI.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userTableObj",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    firstName = table.Column<string>(type: "text", nullable: true),
                    lastName = table.Column<string>(type: "text", nullable: true),
                    username = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true),
                    roles = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userTableObj", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userTableObj");
        }
    }
}
