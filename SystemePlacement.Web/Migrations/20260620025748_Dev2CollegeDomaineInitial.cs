using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev2CollegeDomaineInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "COLLEGE",
                columns: table => new
                {
                    IdCollege = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ville = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COLLEGE", x => x.IdCollege);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                        principalColumn: "IdCollege",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DOMAINE_ETUDE_id_college",
                table: "DOMAINE_ETUDE",
                column: "id_college");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOMAINE_ETUDE");

            migrationBuilder.DropTable(
                name: "COLLEGE");
        }
    }
}
