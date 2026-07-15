using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AjoutThemeCollege : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "couleur_accent",
                table: "colleges",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#69be28")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "couleur_fond",
                table: "colleges",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#f4f7fb")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "couleur_primaire",
                table: "colleges",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#009fda")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "couleur_primaire_foncee",
                table: "colleges",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#003f7d")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "couleur_secondaire",
                table: "colleges",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#0053a1")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "couleur_texte",
                table: "colleges",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#172033")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "logo_url",
                table: "colleges",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "couleur_accent",
                table: "colleges");

            migrationBuilder.DropColumn(
                name: "couleur_fond",
                table: "colleges");

            migrationBuilder.DropColumn(
                name: "couleur_primaire",
                table: "colleges");

            migrationBuilder.DropColumn(
                name: "couleur_primaire_foncee",
                table: "colleges");

            migrationBuilder.DropColumn(
                name: "couleur_secondaire",
                table: "colleges");

            migrationBuilder.DropColumn(
                name: "couleur_texte",
                table: "colleges");

            migrationBuilder.DropColumn(
                name: "logo_url",
                table: "colleges");
        }
    }
}
