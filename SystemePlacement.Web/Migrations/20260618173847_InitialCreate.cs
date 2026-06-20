using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "colleges",
                columns: table => new
                {
                    id_college = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ville = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colleges", x => x.id_college);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_role = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nom_role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_role);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "utilisateurs",
                columns: table => new
                {
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    prenom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    courriel = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nom_utilisateur = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mot_de_passe_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    langue = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "fr")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    date_creation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modification = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    derniere_connexion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    id_role = table.Column<int>(type: "int", nullable: false),
                    id_college = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateurs", x => x.id_utilisateur);
                    table.ForeignKey(
                        name: "FK_utilisateurs_colleges_id_college",
                        column: x => x.id_college,
                        principalTable: "colleges",
                        principalColumn: "id_college",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_utilisateurs_roles_id_role",
                        column: x => x.id_role,
                        principalTable: "roles",
                        principalColumn: "id_role",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "administrateurs",
                columns: table => new
                {
                    id_administrateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    niveau_acces = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Standard")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrateurs", x => x.id_administrateur);
                    table.ForeignKey(
                        name: "FK_administrateurs_utilisateurs_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "employeurs",
                columns: table => new
                {
                    id_employeur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employeurs", x => x.id_employeur);
                    table.ForeignKey(
                        name: "FK_employeurs_utilisateurs_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "etudiants",
                columns: table => new
                {
                    id_etudiant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etudiants", x => x.id_etudiant);
                    table.ForeignKey(
                        name: "FK_etudiants_utilisateurs_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "responsables_stage",
                columns: table => new
                {
                    id_responsable = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responsables_stage", x => x.id_responsable);
                    table.ForeignKey(
                        name: "FK_responsables_stage_utilisateurs_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_administrateurs_id_utilisateur",
                table: "administrateurs",
                column: "id_utilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_colleges_nom",
                table: "colleges",
                column: "nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employeurs_id_utilisateur",
                table: "employeurs",
                column: "id_utilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_etudiants_id_utilisateur",
                table: "etudiants",
                column: "id_utilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_responsables_stage_id_utilisateur",
                table: "responsables_stage",
                column: "id_utilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_nom_role",
                table: "roles",
                column: "nom_role",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_utilisateurs_courriel",
                table: "utilisateurs",
                column: "courriel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_utilisateurs_id_college",
                table: "utilisateurs",
                column: "id_college");

            migrationBuilder.CreateIndex(
                name: "IX_utilisateurs_id_role",
                table: "utilisateurs",
                column: "id_role");

            migrationBuilder.CreateIndex(
                name: "IX_utilisateurs_nom_utilisateur",
                table: "utilisateurs",
                column: "nom_utilisateur",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrateurs");

            migrationBuilder.DropTable(
                name: "employeurs");

            migrationBuilder.DropTable(
                name: "etudiants");

            migrationBuilder.DropTable(
                name: "responsables_stage");

            migrationBuilder.DropTable(
                name: "utilisateurs");

            migrationBuilder.DropTable(
                name: "colleges");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
