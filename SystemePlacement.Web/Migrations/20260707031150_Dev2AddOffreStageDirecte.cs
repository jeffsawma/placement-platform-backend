using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev2AddOffreStageDirecte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "offres_stage_directes",
                columns: table => new
                {
                    id_offre_directe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_etudiant = table.Column<int>(type: "int", nullable: false),
                    id_employeur = table.Column<int>(type: "int", nullable: false),
                    id_offre_stage = table.Column<int>(type: "int", nullable: true),
                    id_candidature = table.Column<int>(type: "int", nullable: true),
                    id_demande_stage = table.Column<int>(type: "int", nullable: true),
                    condtions = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_debut_proposee = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    date_fin_proposee = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    date_proposition = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    statut = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Envoyee")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reponse_etudiant = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    commentaire = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offres_stage_directes", x => x.id_offre_directe);
                    table.ForeignKey(
                        name: "FK_offres_stage_directes_candidatures_id_candidature",
                        column: x => x.id_candidature,
                        principalTable: "candidatures",
                        principalColumn: "id_candidature",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_offres_stage_directes_demandes_stage_id_demande_stage",
                        column: x => x.id_demande_stage,
                        principalTable: "demandes_stage",
                        principalColumn: "id_demande_stage",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_offres_stage_directes_employeurs_id_employeur",
                        column: x => x.id_employeur,
                        principalTable: "employeurs",
                        principalColumn: "id_employeur",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_offres_stage_directes_etudiants_id_etudiant",
                        column: x => x.id_etudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_offres_stage_directes_offres_id_offre_stage",
                        column: x => x.id_offre_stage,
                        principalTable: "offres",
                        principalColumn: "id_offre",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_offres_stage_directes_id_candidature",
                table: "offres_stage_directes",
                column: "id_candidature");

            migrationBuilder.CreateIndex(
                name: "IX_offres_stage_directes_id_demande_stage",
                table: "offres_stage_directes",
                column: "id_demande_stage");

            migrationBuilder.CreateIndex(
                name: "IX_offres_stage_directes_id_employeur",
                table: "offres_stage_directes",
                column: "id_employeur");

            migrationBuilder.CreateIndex(
                name: "IX_offres_stage_directes_id_etudiant",
                table: "offres_stage_directes",
                column: "id_etudiant");

            migrationBuilder.CreateIndex(
                name: "IX_offres_stage_directes_id_offre_stage",
                table: "offres_stage_directes",
                column: "id_offre_stage");

            migrationBuilder.CreateIndex(
                name: "IX_offres_stage_directes_statut",
                table: "offres_stage_directes",
                column: "statut");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "offres_stage_directes");
        }
    }
}
