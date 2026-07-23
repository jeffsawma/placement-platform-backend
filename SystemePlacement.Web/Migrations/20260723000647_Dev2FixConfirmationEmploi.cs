using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev2FixConfirmationEmploi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateConfirmationEmploi",
                table: "candidatures",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmploiConfirme",
                table: "candidatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MessageConfirmationEmploi",
                table: "candidatures",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateConfirmationEmploi",
                table: "candidatures");

            migrationBuilder.DropColumn(
                name: "EmploiConfirme",
                table: "candidatures");

            migrationBuilder.DropColumn(
                name: "MessageConfirmationEmploi",
                table: "candidatures");
        }
    }
}
