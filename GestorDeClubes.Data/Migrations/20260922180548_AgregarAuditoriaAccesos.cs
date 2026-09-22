using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorDeClubes.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAuditoriaAccesos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDITORIA_ACCESOS",
                columns: table => new
                {
                    AuditoriaAccesoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDITORIA_ACCESOS", x => x.AuditoriaAccesoID);
                    table.ForeignKey(
                        name: "FK_AUDITORIA_ACCESOS_USUARIOS_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "USUARIOS",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDITORIA_ACCESOS_FechaHora",
                table: "AUDITORIA_ACCESOS",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_AUDITORIA_ACCESOS_UsuarioID",
                table: "AUDITORIA_ACCESOS",
                column: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDITORIA_ACCESOS");
        }
    }
}
