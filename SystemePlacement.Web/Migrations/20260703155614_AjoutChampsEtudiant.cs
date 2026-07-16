using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AjoutChampsEtudiant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CheminementATE",
                table: "etudiants",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CvUrl",
                table: "etudiants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NumeroEtudiant",
                table: "etudiants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Programme",
                table: "etudiants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StatutEtudiant",
                table: "etudiants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "etudiants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "EtudiantIdEtudiant",
                table: "demandes_stage",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EtudiantIdEtudiant",
                table: "candidatures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_demandes_stage_EtudiantIdEtudiant",
                table: "demandes_stage",
                column: "EtudiantIdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_candidatures_EtudiantIdEtudiant",
                table: "candidatures",
                column: "EtudiantIdEtudiant");

            migrationBuilder.AddForeignKey(
                name: "FK_candidatures_etudiants_EtudiantIdEtudiant",
                table: "candidatures",
                column: "EtudiantIdEtudiant",
                principalTable: "etudiants",
                principalColumn: "id_etudiant");

            migrationBuilder.AddForeignKey(
                name: "FK_demandes_stage_etudiants_EtudiantIdEtudiant",
                table: "demandes_stage",
                column: "EtudiantIdEtudiant",
                principalTable: "etudiants",
                principalColumn: "id_etudiant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_candidatures_etudiants_EtudiantIdEtudiant",
                table: "candidatures");

            migrationBuilder.DropForeignKey(
                name: "FK_demandes_stage_etudiants_EtudiantIdEtudiant",
                table: "demandes_stage");

            migrationBuilder.DropIndex(
                name: "IX_demandes_stage_EtudiantIdEtudiant",
                table: "demandes_stage");

            migrationBuilder.DropIndex(
                name: "IX_candidatures_EtudiantIdEtudiant",
                table: "candidatures");

            migrationBuilder.DropColumn(
                name: "CheminementATE",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "CvUrl",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "NumeroEtudiant",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "Programme",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "StatutEtudiant",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "EtudiantIdEtudiant",
                table: "demandes_stage");

            migrationBuilder.DropColumn(
                name: "EtudiantIdEtudiant",
                table: "candidatures");
        }
    }
}
