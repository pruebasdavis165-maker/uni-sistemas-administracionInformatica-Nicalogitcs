"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { AdminGuard } from "@/components/AdminGuard";
import { ApiError } from "@/services/api/client";
import * as catalogService from "@/services/api/catalogService";
import type { Producto } from "@/types";

export default function InventarioAdminPage() {
  return (
    <AdminGuard>
      <InventarioContent />
    </AdminGuard>
  );
}

function InventarioContent() {
  const [productos, setProductos] = useState<Producto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [savingId, setSavingId] = useState<number | null>(null);

  const load = useCallback(async () => {
    setError(null);
    try {
      const data = await catalogService.getCatalog();
      setProductos(data);
    } catch (e) {
      setError(
        e instanceof ApiError ? e.message : "Error al cargar inventario.",
      );
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  async function handleUpdate(
    e: FormEvent<HTMLFormElement>,
    idProducto: number,
  ) {
    e.preventDefault();
    const form = e.currentTarget;
    const input = form.elements.namedItem("cantidad") as HTMLInputElement;
    const cantidad = parseInt(input.value, 10);
    if (Number.isNaN(cantidad) || cantidad < 0) return;

    setSavingId(idProducto);
    try {
      const updated = await catalogService.updateInventario(idProducto, {
        cantidad,
      });
      setProductos((prev) =>
        prev.map((p) => (p.idProducto === idProducto ? updated : p)),
      );
    } catch (err) {
      alert(
        err instanceof ApiError ? err.message : "No se pudo actualizar stock.",
      );
    } finally {
      setSavingId(null);
    }
  }

  return (
    <div className="mx-auto w-full max-w-5xl flex-1 px-4 py-8">
      <h1 className="text-2xl font-bold text-zinc-900">Inventario</h1>
      <p className="mt-1 text-zinc-600">
        Ajusta existencias de productos en tiempo real.
      </p>

      {loading && (
        <p className="mt-8 text-sm text-zinc-500">Cargando productos…</p>
      )}

      {error && (
        <div className="mt-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {error}
        </div>
      )}

      {!loading && !error && (
        <div className="mt-6 overflow-x-auto rounded-xl border border-zinc-200 bg-white shadow-sm">
          <table className="min-w-full text-left text-sm">
            <thead className="border-b border-zinc-200 bg-zinc-50 text-zinc-600">
              <tr>
                <th className="px-4 py-3 font-medium">Producto</th>
                <th className="px-4 py-3 font-medium">Categoría</th>
                <th className="px-4 py-3 font-medium">Precio</th>
                <th className="px-4 py-3 font-medium">Existencias</th>
                <th className="px-4 py-3 font-medium">Estado</th>
                <th className="px-4 py-3 font-medium">Ajustar</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-100">
              {productos.map((p) => (
                <tr key={p.idProducto}>
                  <td className="px-4 py-3 font-medium text-zinc-900">
                    {p.nombre}
                  </td>
                  <td className="px-4 py-3 text-zinc-600">{p.nombreCategoria}</td>
                  <td className="px-4 py-3">C${p.precio.toFixed(2)}</td>
                  <td className="px-4 py-3">{p.existencias}</td>
                  <td className="px-4 py-3">
                    <span
                      className={
                        p.estado ? "text-emerald-600" : "text-zinc-400"
                      }
                    >
                      {p.estado ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <form
                      className="flex items-center gap-2"
                      onSubmit={(e) => handleUpdate(e, p.idProducto)}
                    >
                      <input
                        name="cantidad"
                        type="number"
                        min={0}
                        defaultValue={p.existencias}
                        className="w-20 rounded border border-zinc-300 px-2 py-1"
                      />
                      <button
                        type="submit"
                        disabled={savingId === p.idProducto}
                        className="rounded bg-sky-600 px-2 py-1 text-xs font-medium text-white hover:bg-sky-700 disabled:opacity-50"
                      >
                        {savingId === p.idProducto ? "…" : "Guardar"}
                      </button>
                    </form>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
