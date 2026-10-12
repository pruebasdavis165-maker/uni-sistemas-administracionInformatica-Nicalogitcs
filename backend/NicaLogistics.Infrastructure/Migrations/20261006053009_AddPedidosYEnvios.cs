using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NicaLogistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPedidosYEnvios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id_pedido = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fecha_pedido = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    id_cliente = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos", x => x.id_pedido);
                    table.ForeignKey(
                        name: "FK_pedidos_usuarios_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detalles_pedido",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pedido = table.Column<int>(type: "integer", nullable: false),
                    id_producto = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalles_pedido", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK_detalles_pedido_pedidos_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedidos",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalles_pedido_productos_id_producto",
                        column: x => x.id_producto,
                        principalTable: "productos",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "envios",
                columns: table => new
                {
                    id_envio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pedido = table.Column<int>(type: "integer", nullable: false),
                    direccion_destino = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    estado_envio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "EnPreparacion"),
                    fecha_estimada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    guia_rastreo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_envios", x => x.id_envio);
                    table.ForeignKey(
                        name: "FK_envios_pedidos_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedidos",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detalles_pedido_id_pedido",
                table: "detalles_pedido",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_detalles_pedido_id_producto",
                table: "detalles_pedido",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_envios_guia_rastreo",
                table: "envios",
                column: "guia_rastreo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_envios_id_pedido",
                table: "envios",
                column: "id_pedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_id_cliente",
                table: "pedidos",
                column: "id_cliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalles_pedido");

            migrationBuilder.DropTable(
                name: "envios");

            migrationBuilder.DropTable(
                name: "pedidos");
        }
    }
}
