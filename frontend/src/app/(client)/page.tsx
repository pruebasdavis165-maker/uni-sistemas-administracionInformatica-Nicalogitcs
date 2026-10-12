"use client";

import { useEffect, useState } from "react";
import { ProductCard } from "@/components/ProductCard";
import { ApiError } from "@/services/api/client";
import * as catalogService from "@/services/api/catalogService";
import type { Producto } from "@/types";

export default function CatalogPage() {
  const [productos, setProductos] = useState<Producto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const data = await catalogService.getCatalog();
        if (!cancelled) {
          setProductos(data.filter((p) => p.estado));
        }
      } catch (e) {
        if (!cancelled) {
          setError(
            e instanceof ApiError
              ? e.message
              : "No se pudo cargar el catálogo.",
          );
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <div className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-zinc-900">Catálogo</h1>
        <p className="mt-1 text-zinc-600">
          Productos disponibles para envío a todo Nicaragua.
        </p>
      </div>

      {loading && (
        <p className="text-center text-sm text-zinc-500">Cargando productos…</p>
      )}

      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {error}
        </div>
      )}

      {!loading && !error && productos.length === 0 && (
        <p className="text-center text-zinc-500">No hay productos activos.</p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {productos.map((p) => (
          <ProductCard key={p.idProducto} producto={p} />
        ))}
      </div>
    </div>
  );
}
