using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notely.Api.Data.Migrations
{
    public partial class AddNoteGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NoteGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "NoteGroupId",
                table: "Notes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoteGroups_UserId",
                table: "NoteGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NoteGroupId",
                table: "Notes",
                column: "NoteGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_NoteGroups_NoteGroupId",
                table: "Notes",
                column: "NoteGroupId",
                principalTable: "NoteGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_NoteGroups_NoteGroupId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_NoteGroupId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "NoteGroupId",
                table: "Notes");

            migrationBuilder.DropTable(name: "NoteGroups");
        }
    }
}
