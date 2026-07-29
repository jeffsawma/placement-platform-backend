using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AjoutEmployeurDestinataireRecommandation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_employeur_destinataire",
                table: "recommandations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_recommandation_employeur_destinataire",
                table: "recommandations",
                column: "id_employeur_destinataire");

            migrationBuilder.AddForeignKey(
                name: "FK_recommandations_employeurs_id_employeur_destinataire",
                table: "recommandations",
                column: "id_employeur_destinataire",
                principalTable: "employeurs",
                principalColumn: "id_employeur",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recommandations_employeurs_id_employeur_destinataire",
                table: "recommandations");

            migrationBuilder.DropIndex(
                name: "idx_recommandation_employeur_destinataire",
                table: "recommandations");

            migrationBuilder.DropColumn(
                name: "id_employeur_destinataire",
                table: "recommandations");
        }
    }
}
