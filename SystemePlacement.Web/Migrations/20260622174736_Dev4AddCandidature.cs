using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev4AddCandidature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CANDIDATURE",
                columns: table => new
                {
                    id_candidature = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_offre = table.Column<int>(type: "int", nullable: false),
                    id_etudiant = table.Column<int>(type: "int", nullable: false),
                    date_candidature = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cv_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lettre_motivation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CANDIDATURE", x => x.id_candidature);
                    table.ForeignKey(
                        name: "FK_CANDIDATURE_etudiants_id_etudiant",
                        column: x => x.id_etudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CANDIDATURE_id_etudiant",
                table: "CANDIDATURE",
                column: "id_etudiant");

            migrationBuilder.CreateIndex(
                name: "IX_CANDIDATURE_id_offre_id_etudiant",
                table: "CANDIDATURE",
                columns: new[] { "id_offre", "id_etudiant" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CANDIDATURE");
        }
    }
}
