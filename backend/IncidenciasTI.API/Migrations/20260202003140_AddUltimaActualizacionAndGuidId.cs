using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidenciasTI.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUltimaActualizacionAndGuidId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "Incidencias",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaActualizacion",
                table: "Incidencias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "Incidencias");

            migrationBuilder.DropColumn(
                name: "UltimaActualizacion",
                table: "Incidencias");
        }
    }
}
