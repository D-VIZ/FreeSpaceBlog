using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeSpace.Migrations
{
    /// <inheritdoc />
    public partial class Plataform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Plataform",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plataform",
                table: "Posts");
        }
    }
}
