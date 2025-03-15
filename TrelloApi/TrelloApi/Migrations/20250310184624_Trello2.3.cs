using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class Trello23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarBackground",
                table: "User",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Label",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Card",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarBackground",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Card");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Label",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }
    }
}
