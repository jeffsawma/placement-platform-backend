using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AjoutDemarchesSuivi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemarchesSuivi",
                columns: table => new
                {
                    IdDemarche = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEtudiant = table.Column<int>(type: "int", nullable: false),
                    EtudiantIdEtudiant = table.Column<int>(type: "int", nullable: true),
                    IdResponsable = table.Column<int>(type: "int", nullable: false),
                    ResponsableIdResponsable = table.Column<int>(type: "int", nullable: true),
                    TypeDemarche = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Note = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDemarche = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemarchesSuivi", x => x.IdDemarche);
                    table.ForeignKey(
                        name: "FK_DemarchesSuivi_etudiants_EtudiantIdEtudiant",
                        column: x => x.EtudiantIdEtudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant");
                    table.ForeignKey(
                        name: "FK_DemarchesSuivi_responsables_stage_ResponsableIdResponsable",
                        column: x => x.ResponsableIdResponsable,
                        principalTable: "responsables_stage",
                        principalColumn: "id_responsable");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DemarchesSuivi_EtudiantIdEtudiant",
                table: "DemarchesSuivi",
                column: "EtudiantIdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_DemarchesSuivi_ResponsableIdResponsable",
                table: "DemarchesSuivi",
                column: "ResponsableIdResponsable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemarchesSuivi");
        }
    }
}
