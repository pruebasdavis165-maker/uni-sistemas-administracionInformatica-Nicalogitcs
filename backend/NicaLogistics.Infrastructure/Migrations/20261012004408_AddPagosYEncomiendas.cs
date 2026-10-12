using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NicaLogistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPagosYEncomiendas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "encomiendas",
                columns: table => new
                {
                    id_encomienda = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pedido = table.Column<int>(type: "integer", nullable: false),
                    origen = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    destino = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Registrada"),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encomiendas", x => x.id_encomienda);
                    table.ForeignKey(
                        name: "FK_encomiendas_pedidos_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedidos",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id_pago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pedido = table.Column<int>(type: "integer", nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Pendiente"),
                    metodo_pago = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_reembolso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.id_pago);
                    table.ForeignKey(
                        name: "FK_pagos_pedidos_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedidos",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_encomiendas_id_pedido",
                table: "encomiendas",
                column: "id_pedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_id_pedido",
                table: "pagos",
                column: "id_pedido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "encomiendas");

            migrationBuilder.DropTable(
                name: "pagos");
        }
    }
}
