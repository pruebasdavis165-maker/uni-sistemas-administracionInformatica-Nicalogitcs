"use client";

import { useCallback, useEffect, useState } from "react";
import { AdminGuard } from "@/components/AdminGuard";
import { PagoBadge } from "@/components/PagoBadge";
import { ApiError } from "@/services/api/client";
import * as paymentService from "@/services/api/paymentService";
import type { Pago } from "@/types";

export default function PagosAdminPage() {
  return (
    <AdminGuard>
      <PagosContent />
    </AdminGuard>
  );
}

function PagosContent() {
  const [pagos, setPagos] = useState<Pago[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reembolsandoId, setReembolsandoId] = useState<number | null>(null);
  const [rowMessage, setRowMessage] = useState<
    Record<number, { ok: boolean; text: string }>
  >({});

  const load = useCallback(async () => {
    setError(null);
    try {
      setPagos(await paymentService.getAllPagos());
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Error al cargar pagos.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  async function handleReembolso(pago: Pago) {
    if (
      !confirm(
        `¿Seguro que deseas procesar el reembolso de C$${pago.monto.toFixed(
          2
        )} para el pedido #${pago.idPedido}? Esta acción cancelará el pedido, cancelará la encomienda y devolverá el inventario.`
      )
    ) {
      return;
    }

    setReembolsandoId(pago.idPago);
    setRowMessage((prev) => {
      const next = { ...prev };
      delete next[pago.idPago];
      return next;
    });

    try {
      const updated = await paymentService.reembolsarPago(pago.idPago);
      setPagos((prev) =>
        prev.map((p) => (p.idPago === updated.idPago ? updated : p))
      );
      setRowMessage((prev) => ({
        ...prev,
        [pago.idPago]: {
          ok: true,
          text: "Reembolso procesado y stock repuesto",
        },
      }));
    } catch (err) {
      setRowMessage((prev) => ({
        ...prev,
        [pago.idPago]: {
          ok: false,
          text: err instanceof ApiError ? err.message : "Error en reembolso.",
        },
      }));
    } finally {
      setReembolsandoId(null);
    }
  }

  return (
    <div className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-zinc-900">
            Control de Pagos y Reembolsos (Módulo 6)
          </h1>
          <p className="mt-1 text-zinc-600">
            Auditoría de transacciones, liquidaciones y procesamiento de reembolsos.
          </p>
        </div>
        <button
          type="button"
          onClick={() => {
            setLoading(true);
            void load();
          }}
          className="rounded-lg border border-zinc-300 bg-white px-3 py-1.5 text-sm font-medium text-zinc-700 hover:bg-zinc-100"
        >
          Recargar
        </button>
      </div>

      {loading && (
        <p className="mt-8 text-sm text-zinc-500">Cargando transacciones de pago…</p>
      )}

      {error && (
        <div className="mt-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {error}
        </div>
      )}

      {!loading && !error && pagos.length === 0 && (
        <div className="mt-8 rounded-xl border border-dashed border-zinc-300 bg-white p-8 text-center">
          <p className="text-zinc-600">No hay pagos registrados aún en el sistema.</p>
        </div>
      )}

      {!loading && !error && pagos.length > 0 && (
        <div className="mt-6 overflow-x-auto rounded-xl border border-zinc-200 bg-white shadow-sm">
          <table className="min-w-full text-left text-sm">
            <thead className="border-b border-zinc-200 bg-zinc-50 text-zinc-600">
              <tr>
                <th className="px-4 py-3 font-medium">ID Pago</th>
                <th className="px-4 py-3 font-medium">Pedido</th>
                <th className="px-4 py-3 font-medium">Fecha</th>
                <th className="px-4 py-3 font-medium">Monto</th>
                <th className="px-4 py-3 font-medium">Método de Pago</th>
                <th className="px-4 py-3 font-medium">Estado</th>
                <th className="px-4 py-3 font-medium">Reembolsado el</th>
                <th className="px-4 py-3 font-medium">Acción</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-100">
              {pagos.map((p) => {
                const msg = rowMessage[p.idPago];
                const puedeReembolsar = p.estado === "Completado";

                return (
                  <tr key={p.idPago} className="hover:bg-zinc-50/50">
                    <td className="px-4 py-3 font-medium text-zinc-900">
                      #{p.idPago}
                    </td>
                    <td className="px-4 py-3 font-semibold text-sky-700">
                      #{p.idPedido}
                    </td>
                    <td className="px-4 py-3 text-zinc-600">
                      {new Date(p.fechaPago).toLocaleString("es-NI", {
                        dateStyle: "short",
                        timeStyle: "short",
                      })}
                    </td>
                    <td className="px-4 py-3 font-bold text-zinc-900">
                      C${p.monto.toFixed(2)}
                    </td>
                    <td className="px-4 py-3 text-xs text-zinc-700">
                      {p.metodoPago}
                    </td>
                    <td className="px-4 py-3">
                      <PagoBadge estado={p.estado} />
                    </td>
                    <td className="px-4 py-3 text-xs text-purple-800 font-medium">
                      {p.fechaReembolso
                        ? new Date(p.fechaReembolso).toLocaleDateString("es-NI")
                        : "—"}
                    </td>
                    <td className="px-4 py-3">
                      {puedeReembolsar ? (
                        <div className="flex flex-col gap-1">
                          <button
                            type="button"
                            disabled={reembolsandoId === p.idPago}
                            onClick={() => void handleReembolso(p)}
                            className="inline-flex items-center justify-center rounded bg-rose-600 px-2.5 py-1 text-xs font-semibold text-white shadow-sm hover:bg-rose-700 disabled:opacity-50 transition"
                          >
                            {reembolsandoId === p.idPago
                              ? "Procesando…"
                              : "↩ Procesar Reembolso"}
                          </button>
                          {msg && (
                            <span
                              className={`text-[11px] ${
                                msg.ok ? "text-emerald-600" : "text-red-600"
                              }`}
                            >
                              {msg.text}
                            </span>
                          )}
                        </div>
                      ) : (
                        <span className="text-xs text-zinc-400 font-medium">
                          {p.estado === "Reembolsado"
                            ? "Reembolsado"
                            : "No aplicable"}
                        </span>
                      )}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
