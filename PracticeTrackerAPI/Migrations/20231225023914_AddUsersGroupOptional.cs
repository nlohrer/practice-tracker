using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersGroupOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Groups",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "Users");

            migrationBuilder.AddColumn<List<string>>(
                name: "Groups",
                table: "Users",
                type: "text[]",
                maxLength: 20,
                nullable: false);
        }
    }
}
