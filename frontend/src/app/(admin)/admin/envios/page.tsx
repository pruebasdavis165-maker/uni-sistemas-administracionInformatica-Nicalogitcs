"use client";

import { useCallback, useEffect, useState } from "react";
import { AdminGuard } from "@/components/AdminGuard";
import { ShippingBadge } from "@/components/ShippingBadge";
import { ApiError } from "@/services/api/client";
import * as orderService from "@/services/api/orderService";
import { ESTADOS_ENVIO, type Pedido } from "@/types";

export default function EnviosAdminPage() {
  return (
    <AdminGuard>
      <EnviosContent />
    </AdminGuard>
  );
}

function EnviosContent() {
  const [pedidos, setPedidos] = useState<Pedido[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [savingId, setSavingId] = useState<number | null>(null);
  const [rowMessage, setRowMessage] = useState<
    Record<number, { ok: boolean; text: string }>
  >({});

  const load = useCallback(async () => {
    setError(null);
    try {
      setPedidos(await orderService.getAllPedidos());
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Error al cargar pedidos.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  async function handleChange(pedido: Pedido, nuevoEstado: string) {
    if (!pedido.envio || nuevoEstado === pedido.envio.estadoEnvio) return;

    setSavingId(pedido.idPedido);
    setRowMessage((prev) => {
      const next = { ...prev };
      delete next[pedido.idPedido];
      return next;
    });

    try {
      const updated = await orderService.updateEstadoEnvio(pedido.idPedido, {
        nuevoEstado,
      });
      setPedidos((prev) =>
        prev.map((p) => (p.idPedido === updated.idPedido ? updated : p)),
      );
      setRowMessage((prev) => ({
        ...prev,
        [pedido.idPedido]: { ok: true, text: "Estado actualizado" },
      }));
    } catch (err) {
      setRowMessage((prev) => ({
        ...prev,
        [pedido.idPedido]: {
          ok: false,
          text:
            err instanceof ApiError ? err.message : "No se pudo actualizar.",
        },
      }));
    } finally {
      setSavingId(null);
    }
  }

  return (
    <div className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-zinc-900">Envíos</h1>
          <p className="mt-1 text-zinc-600">
            Actualiza el estado de envío de cada pedido.
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
        <p className="mt-8 text-sm text-zinc-500">Cargando pedidos…</p>
      )}

      {error && (
        <div className="mt-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {error}
        </div>
      )}

      {!loading && !error && pedidos.length === 0 && (
        <p className="mt-8 text-sm text-zinc-500">No hay pedidos registrados.</p>
      )}

      {!loading && !error && pedidos.length > 0 && (
        <div className="mt-6 overflow-x-auto rounded-xl border border-zinc-200 bg-white shadow-sm">
          <table className="min-w-full text-left text-sm">
            <thead className="border-b border-zinc-200 bg-zinc-50 text-zinc-600">
              <tr>
                <th className="px-4 py-3 font-medium">Pedido</th>
                <th className="px-4 py-3 font-medium">Fecha</th>
                <th className="px-4 py-3 font-medium">Total</th>
                <th className="px-4 py-3 font-medium">Estado pedido</th>
                <th className="px-4 py-3 font-medium">Guía</th>
                <th className="px-4 py-3 font-medium">Destino</th>
                <th className="px-4 py-3 font-medium">Envío</th>
                <th className="px-4 py-3 font-medium">Cambiar estado</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-100">
              {pedidos.map((p) => {
                const msg = rowMessage[p.idPedido];
                return (
                  <tr key={p.idPedido}>
                    <td className="px-4 py-3 font-medium text-zinc-900">
                      #{p.idPedido}
                    </td>
                    <td className="px-4 py-3 text-zinc-600">
                      {new Date(p.fechaPedido).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-3">C${p.total.toFixed(2)}</td>
                    <td className="px-4 py-3">{p.estado}</td>
                    <td className="px-4 py-3 font-mono text-xs">
                      {p.envio?.guiaRastreo ?? "—"}
                    </td>
                    <td className="max-w-[14rem] truncate px-4 py-3 text-zinc-600">
                      {p.envio?.direccionDestino ?? "—"}
                    </td>
                    <td className="px-4 py-3">
                      {p.envio ? (
                        <ShippingBadge estado={p.envio.estadoEnvio} />
                      ) : (
                        <span className="text-zinc-400">Sin envío</span>
                      )}
                    </td>
                    <td className="px-4 py-3">
                      {p.envio ? (
                        <div className="flex flex-col gap-1">
                          <select
                            value={p.envio.estadoEnvio}
                            disabled={savingId === p.idPedido}
                            onChange={(e) => void handleChange(p, e.target.value)}
                            className="rounded border border-zinc-300 bg-white px-2 py-1 disabled:opacity-50"
                          >
                            {ESTADOS_ENVIO.map((estado) => (
                              <option key={estado} value={estado}>
                                {estado}
                              </option>
                            ))}
                          </select>
                          {msg && (
                            <span
                              className={`text-xs ${msg.ok ? "text-emerald-600" : "text-red-600"}`}
                            >
                              {msg.text}
                            </span>
                          )}
                        </div>
                      ) : (
                        <span className="text-zinc-400">—</span>
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
