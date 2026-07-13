using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemePlacement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Dev4AddNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id_notification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lue = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    date_creation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id_notification);
                    table.ForeignKey(
                        name: "FK_notifications_utilisateurs_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_id_utilisateur",
                table: "notifications",
                column: "id_utilisateur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications");
        }
    }
}
