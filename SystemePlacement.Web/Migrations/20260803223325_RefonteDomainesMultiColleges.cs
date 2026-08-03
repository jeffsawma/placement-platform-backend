using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class RefonteDomainesMultiColleges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_domaine_etudes_colleges_id_college",
                table: "domaine_etudes");

            migrationBuilder.DropIndex(
                name: "IX_domaine_etudes_id_college",
                table: "domaine_etudes");

            migrationBuilder.DropColumn(
                name: "accepte_stagiaires",
                table: "domaine_etudes");

            migrationBuilder.DropColumn(
                name: "id_college",
                table: "domaine_etudes");

            migrationBuilder.AlterColumn<bool>(
                name: "actif",
                table: "domaine_etudes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.CreateTable(
                name: "college_domaines",
                columns: table => new
                {
                    id_college_domaine = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_college = table.Column<int>(type: "int", nullable: false),
                    id_domaine = table.Column<int>(type: "int", nullable: false),
                    accepte_stagiaires = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_college_domaines", x => x.id_college_domaine);
                    table.ForeignKey(
                        name: "FK_college_domaines_colleges_id_college",
                        column: x => x.id_college,
                        principalTable: "colleges",
                        principalColumn: "id_college",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_college_domaines_domaine_etudes_id_domaine",
                        column: x => x.id_domaine,
                        principalTable: "domaine_etudes",
                        principalColumn: "id_domaine",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_domaine_etudes_code",
                table: "domaine_etudes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_domaine_etudes_nom",
                table: "domaine_etudes",
                column: "nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_college_domaines_id_college_id_domaine",
                table: "college_domaines",
                columns: new[] { "id_college", "id_domaine" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_college_domaines_id_domaine",
                table: "college_domaines",
                column: "id_domaine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "college_domaines");

            migrationBuilder.DropIndex(
                name: "IX_domaine_etudes_code",
                table: "domaine_etudes");

            migrationBuilder.DropIndex(
                name: "IX_domaine_etudes_nom",
                table: "domaine_etudes");

            migrationBuilder.AlterColumn<bool>(
                name: "actif",
                table: "domaine_etudes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "accepte_stagiaires",
                table: "domaine_etudes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "id_college",
                table: "domaine_etudes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_domaine_etudes_id_college",
                table: "domaine_etudes",
                column: "id_college");

            migrationBuilder.AddForeignKey(
                name: "FK_domaine_etudes_colleges_id_college",
                table: "domaine_etudes",
                column: "id_college",
                principalTable: "colleges",
                principalColumn: "id_college",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
