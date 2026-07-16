using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class CorrectionRelationsStagesConfirmations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmationsStage_Stages_StageIdStage",
                table: "ConfirmationsStage");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmationsStage_utilisateurs_UtilisateurIdUtilisateur",
                table: "ConfirmationsStage");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_etudiants_EtudiantIdEtudiant",
                table: "Stages");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_offres_OffreIdOffre",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_Stages_EtudiantIdEtudiant",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_Stages_OffreIdOffre",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmationsStage_StageIdStage",
                table: "ConfirmationsStage");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmationsStage_UtilisateurIdUtilisateur",
                table: "ConfirmationsStage");

            migrationBuilder.DropColumn(
                name: "EtudiantIdEtudiant",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "OffreIdOffre",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "StageIdStage",
                table: "ConfirmationsStage");

            migrationBuilder.DropColumn(
                name: "UtilisateurIdUtilisateur",
                table: "ConfirmationsStage");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_IdEtudiant",
                table: "Stages",
                column: "IdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_IdOffre",
                table: "Stages",
                column: "IdOffre");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationsStage_IdStage",
                table: "ConfirmationsStage",
                column: "IdStage");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationsStage_IdUtilisateur",
                table: "ConfirmationsStage",
                column: "IdUtilisateur");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfirmationsStage_Stages_IdStage",
                table: "ConfirmationsStage",
                column: "IdStage",
                principalTable: "Stages",
                principalColumn: "IdStage",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfirmationsStage_utilisateurs_IdUtilisateur",
                table: "ConfirmationsStage",
                column: "IdUtilisateur",
                principalTable: "utilisateurs",
                principalColumn: "id_utilisateur",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_etudiants_IdEtudiant",
                table: "Stages",
                column: "IdEtudiant",
                principalTable: "etudiants",
                principalColumn: "id_etudiant",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_offres_IdOffre",
                table: "Stages",
                column: "IdOffre",
                principalTable: "offres",
                principalColumn: "id_offre",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmationsStage_Stages_IdStage",
                table: "ConfirmationsStage");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmationsStage_utilisateurs_IdUtilisateur",
                table: "ConfirmationsStage");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_etudiants_IdEtudiant",
                table: "Stages");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_offres_IdOffre",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_Stages_IdEtudiant",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_Stages_IdOffre",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmationsStage_IdStage",
                table: "ConfirmationsStage");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmationsStage_IdUtilisateur",
                table: "ConfirmationsStage");

            migrationBuilder.AddColumn<int>(
                name: "EtudiantIdEtudiant",
                table: "Stages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OffreIdOffre",
                table: "Stages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StageIdStage",
                table: "ConfirmationsStage",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurIdUtilisateur",
                table: "ConfirmationsStage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stages_EtudiantIdEtudiant",
                table: "Stages",
                column: "EtudiantIdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_OffreIdOffre",
                table: "Stages",
                column: "OffreIdOffre");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationsStage_StageIdStage",
                table: "ConfirmationsStage",
                column: "StageIdStage");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationsStage_UtilisateurIdUtilisateur",
                table: "ConfirmationsStage",
                column: "UtilisateurIdUtilisateur");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfirmationsStage_Stages_StageIdStage",
                table: "ConfirmationsStage",
                column: "StageIdStage",
                principalTable: "Stages",
                principalColumn: "IdStage");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfirmationsStage_utilisateurs_UtilisateurIdUtilisateur",
                table: "ConfirmationsStage",
                column: "UtilisateurIdUtilisateur",
                principalTable: "utilisateurs",
                principalColumn: "id_utilisateur");

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_etudiants_EtudiantIdEtudiant",
                table: "Stages",
                column: "EtudiantIdEtudiant",
                principalTable: "etudiants",
                principalColumn: "id_etudiant");

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_offres_OffreIdOffre",
                table: "Stages",
                column: "OffreIdOffre",
                principalTable: "offres",
                principalColumn: "id_offre");
        }
    }
}
