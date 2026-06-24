using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev4AddOffre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OFFRE",
                columns: table => new
                {
                    id_offre = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    titre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lieu = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_publication = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_expiration = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type_offre = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_entreprise = table.Column<int>(type: "int", nullable: false),
                    nombre_postes = table.Column<int>(type: "int", nullable: true),
                    remunere = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OFFRE", x => x.id_offre);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OFFRE_DOMAINE",
                columns: table => new
                {
                    id_offre = table.Column<int>(type: "int", nullable: false),
                    id_domaine = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OFFRE_DOMAINE", x => new { x.id_offre, x.id_domaine });
                    table.ForeignKey(
                        name: "FK_OFFRE_DOMAINE_DOMAINE_ETUDE_id_domaine",
                        column: x => x.id_domaine,
                        principalTable: "DOMAINE_ETUDE",
                        principalColumn: "id_domaine",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OFFRE_DOMAINE_OFFRE_id_offre",
                        column: x => x.id_offre,
                        principalTable: "OFFRE",
                        principalColumn: "id_offre",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OFFRE_DOMAINE_id_domaine",
                table: "OFFRE_DOMAINE",
                column: "id_domaine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OFFRE_DOMAINE");

            migrationBuilder.DropTable(
                name: "OFFRE");
        }
    }
}
