"use client";

import { useCallback, useEffect, useState } from "react";
import { AdminGuard } from "@/components/AdminGuard";
import { EncomiendaBadge } from "@/components/EncomiendaBadge";
import { ApiError } from "@/services/api/client";
import * as encomiendaService from "@/services/api/encomiendaService";
import { ESTADOS_ENCOMIENDA, type Encomienda } from "@/types";

export default function EncomiendasAdminPage() {
  return (
    <AdminGuard>
      <EncomiendasContent />
    </AdminGuard>
  );
}

function EncomiendasContent() {
  const [encomiendas, setEncomiendas] = useState<Encomienda[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [savingId, setSavingId] = useState<number | null>(null);
  const [rowMessage, setRowMessage] = useState<
    Record<number, { ok: boolean; text: string }>
  >({});

  const load = useCallback(async () => {
    setError(null);
    try {
      setEncomiendas(await encomiendaService.getAllEncomiendas());
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Error al cargar encomiendas.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  async function handleChangeEstado(encomienda: Encomienda, nuevoEstado: string) {
    if (nuevoEstado === encomienda.estado) return;

    setSavingId(encomienda.idEncomienda);
    setRowMessage((prev) => {
      const next = { ...prev };
      delete next[encomienda.idEncomienda];
      return next;
    });

    try {
      const updated = await encomiendaService.actualizarEstadoEncomienda(
        encomienda.idEncomienda,
        { nuevoEstado }
      );
      setEncomiendas((prev) =>
        prev.map((e) => (e.idEncomienda === updated.idEncomienda ? updated : e))
      );
      setRowMessage((prev) => ({
        ...prev,
        [encomienda.idEncomienda]: { ok: true, text: "Estado actualizado" },
      }));
    } catch (err) {
      setRowMessage((prev) => ({
        ...prev,
        [encomienda.idEncomienda]: {
          ok: false,
          text: err instanceof ApiError ? err.message : "Error al actualizar.",
        },
      }));
    } finally {
      setSavingId(null);
    }
  }

  return (
    <div className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-zinc-900">
            Control de Encomiendas (Automatización Módulo 5)
          </h1>
          <p className="mt-1 text-zinc-600">
            Encomiendas generadas automáticamente tras la confirmación de pagos.
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
        <p className="mt-8 text-sm text-zinc-500">Cargando encomiendas…</p>
      )}

      {error && (
        <div className="mt-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {error}
        </div>
      )}

      {!loading && !error && encomiendas.length === 0 && (
        <div className="mt-8 rounded-xl border border-dashed border-zinc-300 bg-white p-8 text-center">
          <p className="text-zinc-600">
            No hay encomiendas generadas aún. Se crearán automáticamente cuando los clientes paguen sus pedidos.
          </p>
        </div>
      )}

      {!loading && !error && encomiendas.length > 0 && (
        <div className="mt-6 overflow-x-auto rounded-xl border border-zinc-200 bg-white shadow-sm">
          <table className="min-w-full text-left text-sm">
            <thead className="border-b border-zinc-200 bg-zinc-50 text-zinc-600">
              <tr>
                <th className="px-4 py-3 font-medium">ID</th>
                <th className="px-4 py-3 font-medium">Pedido</th>
                <th className="px-4 py-3 font-medium">Registro</th>
                <th className="px-4 py-3 font-medium">Origen</th>
                <th className="px-4 py-3 font-medium">Destino</th>
                <th className="px-4 py-3 font-medium">Guía Rastreo</th>
                <th className="px-4 py-3 font-medium">Estado</th>
                <th className="px-4 py-3 font-medium">Acción</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-100">
              {encomiendas.map((e) => {
                const msg = rowMessage[e.idEncomienda];
                const esFinal = e.estado === "Entregada" || e.estado === "Cancelada";

                return (
                  <tr key={e.idEncomienda} className="hover:bg-zinc-50/50">
                    <td className="px-4 py-3 font-medium text-zinc-900">
                      #{e.idEncomienda}
                    </td>
                    <td className="px-4 py-3 font-semibold text-sky-700">
                      #{e.idPedido}
                    </td>
                    <td className="px-4 py-3 text-zinc-600">
                      {new Date(e.fechaRegistro).toLocaleDateString()}
                    </td>
                    <td className="max-w-[12rem] truncate px-4 py-3 text-zinc-600 text-xs">
                      {e.origen}
                    </td>
                    <td className="max-w-[14rem] truncate px-4 py-3 text-zinc-700 text-xs font-medium">
                      {e.destino}
                    </td>
                    <td className="px-4 py-3 font-mono text-xs text-zinc-800">
                      {e.guiaRastreo ?? "—"}
                    </td>
                    <td className="px-4 py-3">
                      <EncomiendaBadge estado={e.estado} />
                    </td>
                    <td className="px-4 py-3">
                      {esFinal ? (
                        <span className="text-xs text-zinc-400 font-medium">
                          Estado finalizado
                        </span>
                      ) : (
                        <div className="flex flex-col gap-1">
                          <select
                            value={e.estado}
                            disabled={savingId === e.idEncomienda}
                            onChange={(ev) =>
                              void handleChangeEstado(e, ev.target.value)
                            }
                            className="rounded border border-zinc-300 bg-white px-2 py-1 text-xs disabled:opacity-50"
                          >
                            {ESTADOS_ENCOMIENDA.map((est) => (
                              <option key={est} value={est}>
                                {est}
                              </option>
                            ))}
                          </select>
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
