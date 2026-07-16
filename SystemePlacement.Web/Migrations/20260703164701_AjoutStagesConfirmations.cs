using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AjoutStagesConfirmations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stages",
                columns: table => new
                {
                    IdStage = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEtudiant = table.Column<int>(type: "int", nullable: false),
                    EtudiantIdEtudiant = table.Column<int>(type: "int", nullable: true),
                    IdOffre = table.Column<int>(type: "int", nullable: true),
                    OffreIdOffre = table.Column<int>(type: "int", nullable: true),
                    DateDebut = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DateFin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Lieu = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Superviseur = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Statut = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateConfirmation = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.IdStage);
                    table.ForeignKey(
                        name: "FK_Stages_etudiants_EtudiantIdEtudiant",
                        column: x => x.EtudiantIdEtudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant");
                    table.ForeignKey(
                        name: "FK_Stages_offres_OffreIdOffre",
                        column: x => x.OffreIdOffre,
                        principalTable: "offres",
                        principalColumn: "id_offre");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConfirmationsStage",
                columns: table => new
                {
                    IdConfirmation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdStage = table.Column<int>(type: "int", nullable: false),
                    StageIdStage = table.Column<int>(type: "int", nullable: true),
                    TypeConfirmation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Decision = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Motif = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDecision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    UtilisateurIdUtilisateur = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationsStage", x => x.IdConfirmation);
                    table.ForeignKey(
                        name: "FK_ConfirmationsStage_Stages_StageIdStage",
                        column: x => x.StageIdStage,
                        principalTable: "Stages",
                        principalColumn: "IdStage");
                    table.ForeignKey(
                        name: "FK_ConfirmationsStage_utilisateurs_UtilisateurIdUtilisateur",
                        column: x => x.UtilisateurIdUtilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationsStage_StageIdStage",
                table: "ConfirmationsStage",
                column: "StageIdStage");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationsStage_UtilisateurIdUtilisateur",
                table: "ConfirmationsStage",
                column: "UtilisateurIdUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_EtudiantIdEtudiant",
                table: "Stages",
                column: "EtudiantIdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_OffreIdOffre",
                table: "Stages",
                column: "OffreIdOffre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmationsStage");

            migrationBuilder.DropTable(
                name: "Stages");
        }
    }
}
