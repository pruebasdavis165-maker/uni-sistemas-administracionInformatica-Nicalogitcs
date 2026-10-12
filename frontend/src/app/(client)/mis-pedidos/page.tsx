"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { EncomiendaBadge } from "@/components/EncomiendaBadge";
import { PagoBadge } from "@/components/PagoBadge";
import { ShippingBadge } from "@/components/ShippingBadge";
import { useAuth } from "@/context/AuthContext";
import { ApiError } from "@/services/api/client";
import * as encomiendaService from "@/services/api/encomiendaService";
import * as orderService from "@/services/api/orderService";
import * as paymentService from "@/services/api/paymentService";
import type { Encomienda, Pago, Pedido } from "@/types";

function formatDate(iso: string) {
  return new Date(iso).toLocaleString("es-NI", {
    dateStyle: "medium",
    timeStyle: "short",
  });
}

const METODOS_PAGO = [
  "Tarjeta de Crédito / Débito (Visa/Mastercard)",
  "Transferencia Bancaria (BAC Credomatic)",
  "Billetera Móvil (Banpro Billetera / Kash)",
];

export default function MisPedidosPage() {
  const { isAuthenticated, isLoading } = useAuth();
  const router = useRouter();
  const [pedidos, setPedidos] = useState<Pedido[]>([]);
  const [pagos, setPagos] = useState<Record<number, Pago>>({});
  const [encomiendas, setEncomiendas] = useState<Record<number, Encomienda>>({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Estado para el modal de pago
  const [pedidoAPagar, setPedidoAPagar] = useState<Pedido | null>(null);
  const [metodoSeleccionado, setMetodoSeleccionado] = useState(METODOS_PAGO[0]);
  const [procesandoPago, setProcesandoPago] = useState(false);
  const [mensajePago, setMensajePago] = useState<{ tipo: "exito" | "error"; texto: string } | null>(null);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.replace("/login");
    }
  }, [isLoading, isAuthenticated, router]);

  const cargarDatos = async () => {
    try {
      setError(null);
      const [pedidosData, pagosData, encomiendasData] = await Promise.all([
        orderService.getMisPedidos(),
        paymentService.getMisPagos().catch(() => [] as Pago[]),
        encomiendaService.getEncomiendasPropias().catch(() => [] as Encomienda[]),
      ]);

      setPedidos(pedidosData);

      const pagosMap: Record<number, Pago> = {};
      pagosData.forEach((p) => {
        pagosMap[p.idPedido] = p;
      });
      setPagos(pagosMap);

      const encomiendasMap: Record<number, Encomienda> = {};
      encomiendasData.forEach((e) => {
        encomiendasMap[e.idPedido] = e;
      });
      setEncomiendas(encomiendasMap);
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Error al cargar pedidos.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isLoading || !isAuthenticated) return;
    void cargarDatos();
  }, [isLoading, isAuthenticated]);

  async function handleConfirmarPago(e: React.FormEvent) {
    e.preventDefault();
    if (!pedidoAPagar) return;

    setProcesandoPago(true);
    setMensajePago(null);

    try {
      const resp = await paymentService.registrarPago({
        idPedido: pedidoAPagar.idPedido,
        metodoPago: metodoSeleccionado,
      });

      setMensajePago({
        tipo: "exito",
        texto: `¡Pago de C$${resp.pago.monto.toFixed(2)} procesado con éxito! Se ha generado automáticamente la Encomienda #${resp.encomienda.idEncomienda}.`,
      });

      // Recargar datos para reflejar nuevo estado
      await cargarDatos();

      setTimeout(() => {
        setPedidoAPagar(null);
        setMensajePago(null);
      }, 2000);
    } catch (err) {
      setMensajePago({
        tipo: "error",
        texto: err instanceof ApiError ? err.message : "Error al procesar el pago.",
      });
    } finally {
      setProcesandoPago(false);
    }
  }

  if (isLoading || loading) {
    return (
      <div className="flex flex-1 items-center justify-center py-24 text-sm text-zinc-500">
        Cargando pedidos e historial logístico…
      </div>
    );
  }

  return (
    <div className="mx-auto w-full max-w-5xl flex-1 px-4 py-8">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-zinc-900">Mis pedidos y seguimiento</h1>
          <p className="mt-1 text-zinc-600">
            Historial de compras, estado de pagos y automatización de encomiendas.
          </p>
        </div>
        <button
          onClick={() => {
            setLoading(true);
            void cargarDatos();
          }}
          className="rounded-lg border border-zinc-300 bg-white px-3 py-1.5 text-sm font-medium text-zinc-700 hover:bg-zinc-50"
        >
          Actualizar
        </button>
      </div>

      {error && (
        <div className="mt-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {error}
        </div>
      )}

      {!error && pedidos.length === 0 && (
        <div className="mt-8 rounded-xl border border-dashed border-zinc-300 bg-white p-8 text-center">
          <p className="text-zinc-600">Aún no tienes pedidos registrados.</p>
          <Link href="/" className="mt-4 inline-block text-sky-700 underline font-medium">
            Explorar catálogo
          </Link>
        </div>
      )}

      <div className="mt-6 space-y-6">
        {pedidos.map((pedido) => {
          const pago = pagos[pedido.idPedido];
          const encomienda = encomiendas[pedido.idPedido];
          const puedePagar = pedido.estado === "Pendiente" && (!pago || pago.estado === "Cancelado");

          return (
            <article
              key={pedido.idPedido}
              className="rounded-xl border border-zinc-200 bg-white p-6 shadow-sm transition hover:shadow-md"
            >
              {/* Encabezado del Pedido */}
              <div className="flex flex-wrap items-start justify-between gap-4 border-b border-zinc-100 pb-4">
                <div>
                  <div className="flex items-center gap-3">
                    <span className="font-bold text-zinc-900 text-lg">
                      Pedido #{pedido.idPedido}
                    </span>
                    <span className="inline-flex items-center rounded-md bg-zinc-100 px-2 py-0.5 text-xs font-medium text-zinc-800">
                      Estado: {pedido.estado}
                    </span>
                  </div>
                  <p className="mt-1 text-xs text-zinc-500">
                    Fecha: {formatDate(pedido.fechaPedido)}
                  </p>
                </div>

                <div className="text-right">
                  <p className="text-lg font-bold text-zinc-900">
                    Total: C${pedido.total.toFixed(2)}
                  </p>
                  {puedePagar && (
                    <button
                      onClick={() => setPedidoAPagar(pedido)}
                      className="mt-2 inline-flex items-center gap-1.5 rounded-lg bg-emerald-600 px-3.5 py-1.5 text-xs font-semibold text-white shadow-sm hover:bg-emerald-700 transition"
                    >
                      💳 Pagar ahora (C${pedido.total.toFixed(2)})
                    </button>
                  )}
                </div>
              </div>

              {/* Módulo de Pagos y Estado */}
              <div className="mt-4 grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="rounded-lg bg-zinc-50 p-3.5 border border-zinc-200/60">
                  <p className="text-xs font-semibold uppercase tracking-wider text-zinc-500">
                    Información de Pago
                  </p>
                  {pago ? (
                    <div className="mt-2 space-y-1 text-sm">
                      <div className="flex items-center justify-between">
                        <span className="text-zinc-600">Estado de pago:</span>
                        <PagoBadge estado={pago.estado} />
                      </div>
                      <p className="text-xs text-zinc-600">
                        <span className="font-medium">Método:</span> {pago.metodoPago}
                      </p>
                      <p className="text-xs text-zinc-500">
                        <span className="font-medium">Registrado:</span> {formatDate(pago.fechaPago)}
                      </p>
                      {pago.fechaReembolso && (
                        <p className="text-xs text-purple-700 font-medium">
                          Reembolsado: {formatDate(pago.fechaReembolso)}
                        </p>
                      )}
                    </div>
                  ) : (
                    <div className="mt-2 flex items-center justify-between">
                      <span className="text-xs text-amber-700 font-medium">
                        Pendiente de pago
                      </span>
                      {puedePagar && (
                        <button
                          onClick={() => setPedidoAPagar(pedido)}
                          className="text-xs font-semibold text-emerald-700 underline"
                        >
                          Pagar
                        </button>
                      )}
                    </div>
                  )}
                </div>

                {/* Módulo de Encomienda Automatizada */}
                <div className="rounded-lg bg-sky-50/50 p-3.5 border border-sky-100">
                  <p className="text-xs font-semibold uppercase tracking-wider text-sky-800">
                    Automatización de Encomienda (Módulo 5)
                  </p>
                  {encomienda ? (
                    <div className="mt-2 space-y-1.5 text-sm">
                      <div className="flex items-center justify-between">
                        <span className="text-xs font-medium text-zinc-700">
                          Encomienda #{encomienda.idEncomienda}
                        </span>
                        <EncomiendaBadge estado={encomienda.estado} />
                      </div>
                      <p className="text-xs text-zinc-600">
                        <span className="font-medium">Origen:</span> {encomienda.origen}
                      </p>
                      <p className="text-xs text-zinc-600 truncate">
                        <span className="font-medium">Destino:</span> {encomienda.destino}
                      </p>
                      {pedido.envio?.guiaRastreo && (
                        <p className="font-mono text-xs text-sky-800 font-semibold">
                          Guía: {pedido.envio.guiaRastreo}
                        </p>
                      )}
                    </div>
                  ) : (
                    <div className="mt-2">
                      <p className="text-xs text-zinc-500">
                        La encomienda se genera automáticamente de forma inmediata al confirmar el pago del pedido.
                      </p>
                    </div>
                  )}
                </div>
              </div>

              {/* Trazabilidad de Envío Tradicional si existe */}
              {pedido.envio && (
                <div className="mt-4 flex flex-wrap items-center justify-between rounded-lg bg-zinc-50 p-3 text-xs border border-zinc-200/60">
                  <div className="flex items-center gap-2">
                    <span className="font-medium text-zinc-700">Envío logístico:</span>
                    <ShippingBadge estado={pedido.envio.estadoEnvio} />
                  </div>
                  <div className="text-zinc-600">
                    <span className="font-medium">Destino de entrega:</span> {pedido.envio.direccionDestino}
                  </div>
                </div>
              )}

              {/* Detalle de Artículos */}
              <div className="mt-4 border-t border-zinc-100 pt-3">
                <p className="text-xs font-semibold text-zinc-500 mb-2">Artículos incluidos:</p>
                <ul className="divide-y divide-zinc-50 text-sm text-zinc-600">
                  {pedido.detalles.map((d) => (
                    <li key={d.idDetalle} className="py-1 flex justify-between">
                      <span>
                        {d.cantidad}× {d.nombreProducto}
                      </span>
                      <span className="font-medium text-zinc-800">
                        C${d.subtotal.toFixed(2)}
                      </span>
                    </li>
                  ))}
                </ul>
              </div>
            </article>
          );
        })}
      </div>

      {/* Modal de Pago */}
      {pedidoAPagar && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-2xl animate-in fade-in zoom-in-95 duration-200">
            <h2 className="text-xl font-bold text-zinc-900">
              Pagar Pedido #{pedidoAPagar.idPedido}
            </h2>
            <p className="mt-1 text-sm text-zinc-600">
              Total a pagar: <strong className="text-zinc-900">C${pedidoAPagar.total.toFixed(2)}</strong>
            </p>

            <form onSubmit={handleConfirmarPago} className="mt-5 space-y-4">
              <div>
                <label className="block text-xs font-semibold text-zinc-700 mb-1">
                  Selecciona el método de pago:
                </label>
                <div className="space-y-2">
                  {METODOS_PAGO.map((metodo) => (
                    <label
                      key={metodo}
                      className={`flex items-center gap-3 rounded-lg border p-3 cursor-pointer text-sm transition ${
                        metodoSeleccionado === metodo
                          ? "border-sky-600 bg-sky-50/50 text-sky-950 font-medium"
                          : "border-zinc-200 hover:bg-zinc-50 text-zinc-700"
                      }`}
                    >
                      <input
                        type="radio"
                        name="metodoPago"
                        value={metodo}
                        checked={metodoSeleccionado === metodo}
                        onChange={(e) => setMetodoSeleccionado(e.target.value)}
                        className="text-sky-600 focus:ring-sky-500"
                      />
                      <span>{metodo}</span>
                    </label>
                  ))}
                </div>
              </div>

              {mensajePago && (
                <div
                  className={`rounded-lg p-3 text-xs font-medium ${
                    mensajePago.tipo === "exito"
                      ? "bg-emerald-50 text-emerald-800 border border-emerald-200"
                      : "bg-red-50 text-red-800 border border-red-200"
                  }`}
                >
                  {mensajePago.texto}
                </div>
              )}

              <div className="flex items-center justify-end gap-3 pt-3">
                <button
                  type="button"
                  disabled={procesandoPago}
                  onClick={() => {
                    setPedidoAPagar(null);
                    setMensajePago(null);
                  }}
                  className="rounded-lg border border-zinc-300 px-4 py-2 text-sm font-medium text-zinc-700 hover:bg-zinc-100 disabled:opacity-50"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={procesandoPago}
                  className="inline-flex items-center gap-2 rounded-lg bg-sky-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-sky-700 disabled:opacity-50"
                >
                  {procesandoPago ? "Procesando pago…" : "Confirmar y Generar Encomienda"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
