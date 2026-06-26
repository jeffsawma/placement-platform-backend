using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddOffresEtDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "statut",
                table: "candidatures",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "EnAttente",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "cv_url",
                table: "candidatures",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "message_motivation",
                table: "candidatures",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "candidature_documents",
                columns: table => new
                {
                    id_document = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_candidature = table.Column<int>(type: "int", nullable: false),
                    type_document = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nom_fichier = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chemin_fichier = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    taille_fichier = table.Column<long>(type: "bigint", nullable: false),
                    date_upload = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidature_documents", x => x.id_document);
                    table.ForeignKey(
                        name: "FK_candidature_documents_candidatures_id_candidature",
                        column: x => x.id_candidature,
                        principalTable: "candidatures",
                        principalColumn: "id_candidature",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "offres",
                columns: table => new
                {
                    id_offre = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    titre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ville = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    adresse = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type_offre = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_publication = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_expiration = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    id_employeur = table.Column<int>(type: "int", nullable: false),
                    type_contrat = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    salaire_min = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    salaire_max = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    tele_travail = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_debut_stage = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    date_fin_stage = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    duree_heures_semaine = table.Column<int>(type: "int", nullable: true),
                    remuneration = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    session = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offres", x => x.id_offre);
                    table.ForeignKey(
                        name: "FK_offres_employeurs_id_employeur",
                        column: x => x.id_employeur,
                        principalTable: "employeurs",
                        principalColumn: "id_employeur",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "offre_domaines",
                columns: table => new
                {
                    id_offre = table.Column<int>(type: "int", nullable: false),
                    id_domaine = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offre_domaines", x => new { x.id_offre, x.id_domaine });
                    table.ForeignKey(
                        name: "FK_offre_domaines_domaine_etudes_id_domaine",
                        column: x => x.id_domaine,
                        principalTable: "domaine_etudes",
                        principalColumn: "id_domaine",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_offre_domaines_offres_id_offre",
                        column: x => x.id_offre,
                        principalTable: "offres",
                        principalColumn: "id_offre",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_candidature_documents_id_candidature",
                table: "candidature_documents",
                column: "id_candidature");

            migrationBuilder.CreateIndex(
                name: "IX_offre_domaines_id_domaine",
                table: "offre_domaines",
                column: "id_domaine");

            migrationBuilder.CreateIndex(
                name: "idx_offre_employeur",
                table: "offres",
                column: "id_employeur");

            migrationBuilder.CreateIndex(
                name: "idx_offre_statut",
                table: "offres",
                column: "statut");

            migrationBuilder.AddForeignKey(
                name: "FK_candidatures_offres_id_offre",
                table: "candidatures",
                column: "id_offre",
                principalTable: "offres",
                principalColumn: "id_offre",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_candidatures_offres_id_offre",
                table: "candidatures");

            migrationBuilder.DropTable(
                name: "candidature_documents");

            migrationBuilder.DropTable(
                name: "offre_domaines");

            migrationBuilder.DropTable(
                name: "offres");

            migrationBuilder.DropColumn(
                name: "message_motivation",
                table: "candidatures");

            migrationBuilder.AlterColumn<string>(
                name: "statut",
                table: "candidatures",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30,
                oldDefaultValue: "EnAttente")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "cv_url",
                table: "candidatures",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
