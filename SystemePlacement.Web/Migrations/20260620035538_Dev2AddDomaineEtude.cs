using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev2AddDomaineEtude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_utilisateurs_colleges_id_college",
                table: "utilisateurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_colleges",
                table: "colleges");

            migrationBuilder.RenameTable(
                name: "colleges",
                newName: "COLLEGE");

            migrationBuilder.RenameIndex(
                name: "IX_colleges_nom",
                table: "COLLEGE",
                newName: "IX_COLLEGE_nom");

            migrationBuilder.AddPrimaryKey(
                name: "PK_COLLEGE",
                table: "COLLEGE",
                column: "id_college");

            migrationBuilder.CreateTable(
                name: "DOMAINE_ETUDE",
                columns: table => new
                {
                    id_domaine = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_college = table.Column<int>(type: "int", nullable: false),
                    nom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    accepte_stagiaires = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOMAINE_ETUDE", x => x.id_domaine);
                    table.ForeignKey(
                        name: "FK_DOMAINE_ETUDE_COLLEGE_id_college",
                        column: x => x.id_college,
                        principalTable: "COLLEGE",
                        principalColumn: "id_college",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DOMAINE_ETUDE_id_college",
                table: "DOMAINE_ETUDE",
                column: "id_college");

            migrationBuilder.AddForeignKey(
                name: "FK_utilisateurs_COLLEGE_id_college",
                table: "utilisateurs",
                column: "id_college",
                principalTable: "COLLEGE",
                principalColumn: "id_college",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_utilisateurs_COLLEGE_id_college",
                table: "utilisateurs");

            migrationBuilder.DropTable(
                name: "DOMAINE_ETUDE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_COLLEGE",
                table: "COLLEGE");

            migrationBuilder.RenameTable(
                name: "COLLEGE",
                newName: "colleges");

            migrationBuilder.RenameIndex(
                name: "IX_COLLEGE_nom",
                table: "colleges",
                newName: "IX_colleges_nom");

            migrationBuilder.AddPrimaryKey(
                name: "PK_colleges",
                table: "colleges",
                column: "id_college");

            migrationBuilder.AddForeignKey(
                name: "FK_utilisateurs_colleges_id_college",
                table: "utilisateurs",
                column: "id_college",
                principalTable: "colleges",
                principalColumn: "id_college",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
