using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeSpace.Migrations
{
    /// <inheritdoc />
    public partial class addtagpath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagPath",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagPath",
                table: "Posts");
        }
    }
}
