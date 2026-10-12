"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { ShippingBadge } from "@/components/ShippingBadge";
import { useAuth } from "@/context/AuthContext";
import { ApiError } from "@/services/api/client";
import * as orderService from "@/services/api/orderService";
import type { Pedido } from "@/types";

function formatDate(iso: string) {
  return new Date(iso).toLocaleString("es-NI", {
    dateStyle: "medium",
    timeStyle: "short",
  });
}

export default function MisPedidosPage() {
  const { isAuthenticated, isLoading } = useAuth();
  const router = useRouter();
  const [pedidos, setPedidos] = useState<Pedido[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.replace("/login");
    }
  }, [isLoading, isAuthenticated, router]);

  useEffect(() => {
    if (isLoading || !isAuthenticated) return;
    let cancelled = false;
    (async () => {
      try {
        const data = await orderService.getMisPedidos();
        if (!cancelled) setPedidos(data);
      } catch (e) {
        if (!cancelled) {
          setError(
            e instanceof ApiError ? e.message : "Error al cargar pedidos.",
          );
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [isLoading, isAuthenticated]);

  if (isLoading || loading) {
    return (
      <div className="flex flex-1 items-center justify-center py-24 text-sm text-zinc-500">
        Cargando pedidos…
      </div>
    );
  }

  return (
    <div className="mx-auto w-full max-w-4xl flex-1 px-4 py-8">
      <h1 className="text-2xl font-bold text-zinc-900">Mis pedidos</h1>
      <p className="mt-1 text-zinc-600">Historial y seguimiento de envíos.</p>

      {error && (
        <div className="mt-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {error}
        </div>
      )}

      {!error && pedidos.length === 0 && (
        <div className="mt-8 rounded-xl border border-dashed border-zinc-300 bg-white p-8 text-center">
          <p className="text-zinc-600">Aún no tienes pedidos.</p>
          <Link href="/" className="mt-4 inline-block text-sky-700 underline">
            Explorar catálogo
          </Link>
        </div>
      )}

      <div className="mt-6 space-y-4">
        {pedidos.map((pedido) => (
          <article
            key={pedido.idPedido}
            className="rounded-xl border border-zinc-200 bg-white p-5 shadow-sm"
          >
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <p className="text-sm text-zinc-500">
                  Pedido #{pedido.idPedido} · {formatDate(pedido.fechaPedido)}
                </p>
                <p className="mt-1 font-semibold text-zinc-900">
                  Total: C${pedido.total.toFixed(2)}
                </p>
                <p className="text-sm text-zinc-600">Estado: {pedido.estado}</p>
              </div>
              {pedido.envio && (
                <div className="text-right">
                  <ShippingBadge estado={pedido.envio.estadoEnvio} />
                  <p className="mt-2 font-mono text-xs text-zinc-600">
                    Guía: {pedido.envio.guiaRastreo || "—"}
                  </p>
                </div>
              )}
            </div>
            <ul className="mt-4 border-t border-zinc-100 pt-3 text-sm text-zinc-600">
              {pedido.detalles.map((d) => (
                <li key={d.idDetalle}>
                  {d.cantidad}× {d.nombreProducto} — C$
                  {d.subtotal.toFixed(2)}
                </li>
              ))}
            </ul>
          </article>
        ))}
      </div>
    </div>
  );
}
