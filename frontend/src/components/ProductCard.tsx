"use client";

import type { Producto } from "@/types";
import { useCart } from "@/context/CartContext";

export function ProductCard({ producto }: { producto: Producto }) {
  const { addItem } = useCart();
  const disponible = producto.estado && producto.existencias > 0;
  const stockBajo = producto.existencias > 0 && producto.existencias <= 5;

  return (
    <article className="flex flex-col overflow-hidden rounded-xl border border-zinc-200 bg-white shadow-sm transition hover:shadow-md">
      <div className="border-b border-zinc-100 bg-gradient-to-br from-sky-50 to-white px-4 py-3">
        <p className="text-xs font-medium uppercase tracking-wide text-sky-700">
          {producto.nombreCategoria}
        </p>
        <h3 className="mt-1 text-lg font-semibold text-zinc-900">
          {producto.nombre}
        </h3>
      </div>
      <div className="flex flex-1 flex-col gap-3 p-4">
        <p className="line-clamp-3 flex-1 text-sm text-zinc-600">
          {producto.descripcion}
        </p>
        <div className="flex items-end justify-between gap-2">
          <div>
            <p className="text-xl font-bold text-zinc-900">
              C${producto.precio.toFixed(2)}
            </p>
            <p
              className={`text-xs font-medium ${
                !disponible
                  ? "text-red-600"
                  : stockBajo
                    ? "text-amber-600"
                    : "text-emerald-600"
              }`}
            >
              {!disponible
                ? "Sin stock"
                : stockBajo
                  ? `Quedan ${producto.existencias} uds.`
                  : `${producto.existencias} en stock`}
            </p>
          </div>
          <button
            type="button"
            disabled={!disponible}
            onClick={() => addItem(producto)}
            className="rounded-lg bg-sky-600 px-3 py-2 text-sm font-medium text-white transition hover:bg-sky-700 disabled:cursor-not-allowed disabled:bg-zinc-300"
          >
            Agregar
          </button>
        </div>
      </div>
    </article>
  );
}
