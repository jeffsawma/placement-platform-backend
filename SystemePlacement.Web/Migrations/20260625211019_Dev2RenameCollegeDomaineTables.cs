using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev2RenameCollegeDomaineTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOMAINE_ETUDE_COLLEGE_id_college",
                table: "DOMAINE_ETUDE");

            migrationBuilder.DropForeignKey(
                name: "FK_utilisateurs_COLLEGE_id_college",
                table: "utilisateurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DOMAINE_ETUDE",
                table: "DOMAINE_ETUDE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_COLLEGE",
                table: "COLLEGE");

            migrationBuilder.RenameTable(
                name: "DOMAINE_ETUDE",
                newName: "domaine_etudes");

            migrationBuilder.RenameTable(
                name: "COLLEGE",
                newName: "colleges");

            migrationBuilder.RenameIndex(
                name: "IX_DOMAINE_ETUDE_id_college",
                table: "domaine_etudes",
                newName: "IX_domaine_etudes_id_college");

            migrationBuilder.RenameIndex(
                name: "IX_COLLEGE_nom",
                table: "colleges",
                newName: "IX_colleges_nom");

            migrationBuilder.AddPrimaryKey(
                name: "PK_domaine_etudes",
                table: "domaine_etudes",
                column: "id_domaine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_colleges",
                table: "colleges",
                column: "id_college");

            migrationBuilder.AddForeignKey(
                name: "FK_domaine_etudes_colleges_id_college",
                table: "domaine_etudes",
                column: "id_college",
                principalTable: "colleges",
                principalColumn: "id_college",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_utilisateurs_colleges_id_college",
                table: "utilisateurs",
                column: "id_college",
                principalTable: "colleges",
                principalColumn: "id_college",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_domaine_etudes_colleges_id_college",
                table: "domaine_etudes");

            migrationBuilder.DropForeignKey(
                name: "FK_utilisateurs_colleges_id_college",
                table: "utilisateurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_domaine_etudes",
                table: "domaine_etudes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_colleges",
                table: "colleges");

            migrationBuilder.RenameTable(
                name: "domaine_etudes",
                newName: "DOMAINE_ETUDE");

            migrationBuilder.RenameTable(
                name: "colleges",
                newName: "COLLEGE");

            migrationBuilder.RenameIndex(
                name: "IX_domaine_etudes_id_college",
                table: "DOMAINE_ETUDE",
                newName: "IX_DOMAINE_ETUDE_id_college");

            migrationBuilder.RenameIndex(
                name: "IX_colleges_nom",
                table: "COLLEGE",
                newName: "IX_COLLEGE_nom");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DOMAINE_ETUDE",
                table: "DOMAINE_ETUDE",
                column: "id_domaine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_COLLEGE",
                table: "COLLEGE",
                column: "id_college");

            migrationBuilder.AddForeignKey(
                name: "FK_DOMAINE_ETUDE_COLLEGE_id_college",
                table: "DOMAINE_ETUDE",
                column: "id_college",
                principalTable: "COLLEGE",
                principalColumn: "id_college",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_utilisateurs_COLLEGE_id_college",
                table: "utilisateurs",
                column: "id_college",
                principalTable: "COLLEGE",
                principalColumn: "id_college",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
