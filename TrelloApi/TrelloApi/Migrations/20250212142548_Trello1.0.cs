using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class Trello10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Label_Task_TaskId",
                table: "Label");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Label",
                newName: "BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Label_TaskId",
                table: "Label",
                newName: "IX_Label_BoardId");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserBoard",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Theme",
                table: "User",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Task",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Task",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Task",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Task",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Task",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "List",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "List",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "List",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Comment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Theme",
                table: "Board",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Board",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Board",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Board",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Board",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TaskLabel",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    LabelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLabel", x => new { x.TaskId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_TaskLabel_Label_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Label",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskLabel_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTask",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TaskId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTask", x => new { x.UserId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_UserTask_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTask_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLabel_LabelId",
                table: "TaskLabel",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTask_TaskId",
                table: "UserTask",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_AuthorId",
                table: "Comment",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Label_Board_BoardId",
                table: "Label",
                column: "BoardId",
                principalTable: "Board",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_AuthorId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Label_Board_BoardId",
                table: "Label");

            migrationBuilder.DropTable(
                name: "TaskLabel");

            migrationBuilder.DropTable(
                name: "UserTask");

            migrationBuilder.DropIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserBoard");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "List");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "List");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "List");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Board");

            migrationBuilder.RenameColumn(
                name: "BoardId",
                table: "Label",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Label_BoardId",
                table: "Label",
                newName: "IX_Label_TaskId");

            migrationBuilder.AlterColumn<string>(
                name: "Theme",
                table: "User",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "Theme",
                table: "Board",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddForeignKey(
                name: "FK_Label_Task_TaskId",
                table: "Label",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
