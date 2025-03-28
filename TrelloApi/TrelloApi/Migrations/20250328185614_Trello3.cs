using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class Trello3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCard",
                table: "UserCard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBoard",
                table: "UserBoard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardLabel",
                table: "CardLabel");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserBoard");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Board");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserCard",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserCard",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserCard",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserBoard",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserBoard",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserBoard",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CardLabel",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CardLabel",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CardLabel",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCard",
                table: "UserCard",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBoard",
                table: "UserBoard",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardLabel",
                table: "CardLabel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserCard_UserId_CardId",
                table: "UserCard",
                columns: new[] { "UserId", "CardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBoard_UserId_BoardId",
                table: "UserBoard",
                columns: new[] { "UserId", "BoardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardLabel_CardId_LabelId",
                table: "CardLabel",
                columns: new[] { "CardId", "LabelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCard",
                table: "UserCard");

            migrationBuilder.DropIndex(
                name: "IX_UserCard_UserId_CardId",
                table: "UserCard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBoard",
                table: "UserBoard");

            migrationBuilder.DropIndex(
                name: "IX_UserBoard_UserId_BoardId",
                table: "UserBoard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardLabel",
                table: "CardLabel");

            migrationBuilder.DropIndex(
                name: "IX_CardLabel_CardId_LabelId",
                table: "CardLabel");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserCard");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserCard");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserCard");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserBoard");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserBoard");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserBoard");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CardLabel");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CardLabel");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CardLabel");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserBoard",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Card",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Card",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Board",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Board",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Board",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCard",
                table: "UserCard",
                columns: new[] { "UserId", "CardId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBoard",
                table: "UserBoard",
                columns: new[] { "UserId", "BoardId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardLabel",
                table: "CardLabel",
                columns: new[] { "CardId", "LabelId" });
        }
    }
}
