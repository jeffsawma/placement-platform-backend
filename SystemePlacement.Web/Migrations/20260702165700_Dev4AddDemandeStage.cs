using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev4AddDemandeStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "demandes_stage",
                columns: table => new
                {
                    id_demande_stage = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_etudiant = table.Column<int>(type: "int", nullable: false),
                    id_domaine = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    periode_souhaitee = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    competences = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statut = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Ouverte")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_creation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_demandes_stage", x => x.id_demande_stage);
                    table.ForeignKey(
                        name: "FK_demandes_stage_domaine_etudes_id_domaine",
                        column: x => x.id_domaine,
                        principalTable: "domaine_etudes",
                        principalColumn: "id_domaine",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_demandes_stage_etudiants_id_etudiant",
                        column: x => x.id_etudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_demandes_stage_id_domaine",
                table: "demandes_stage",
                column: "id_domaine");

            migrationBuilder.CreateIndex(
                name: "IX_demandes_stage_id_etudiant",
                table: "demandes_stage",
                column: "id_etudiant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "demandes_stage");
        }
    }
}
