"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { useCart } from "@/context/CartContext";
import { ApiError } from "@/services/api/client";
import * as orderService from "@/services/api/orderService";

export default function CheckoutPage() {
  const { isAuthenticated, isLoading } = useAuth();
  const { items, subtotal, updateQuantity, removeItem, clearCart } = useCart();
  const router = useRouter();
  const [direccion, setDireccion] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [successId, setSuccessId] = useState<number | null>(null);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.replace("/login");
    }
  }, [isLoading, isAuthenticated, router]);

  async function handleConfirm(e: FormEvent) {
    e.preventDefault();
    if (items.length === 0) return;
    setError(null);
    setSubmitting(true);
    try {
      const pedido = await orderService.crearPedido({
        items: items.map((i) => ({
          idProducto: i.idProducto,
          cantidad: i.cantidad,
        })),
        direccionDestino: direccion.trim(),
      });
      clearCart();
      setSuccessId(pedido.idPedido);
    } catch (err) {
      setError(
        err instanceof ApiError ? err.message : "No se pudo crear el pedido.",
      );
    } finally {
      setSubmitting(false);
    }
  }

  if (isLoading) {
    return (
      <div className="flex flex-1 items-center justify-center py-24 text-sm text-zinc-500">
        Cargando…
      </div>
    );
  }

  if (successId !== null) {
    return (
      <div className="mx-auto max-w-lg flex-1 px-4 py-16 text-center">
        <h1 className="text-2xl font-bold text-emerald-800">¡Pedido confirmado!</h1>
        <p className="mt-2 text-zinc-600">
          Tu pedido #{successId} fue registrado correctamente.
        </p>
        <Link
          href="/mis-pedidos"
          className="mt-6 inline-block rounded-lg bg-sky-600 px-4 py-2 text-sm font-medium text-white hover:bg-sky-700"
        >
          Ver mis pedidos
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto w-full max-w-3xl flex-1 px-4 py-8">
      <h1 className="text-2xl font-bold text-zinc-900">Checkout</h1>
      <p className="mt-1 text-zinc-600">Revisa tu carrito y confirma el envío.</p>

      {items.length === 0 ? (
        <div className="mt-8 rounded-xl border border-dashed border-zinc-300 bg-white p-8 text-center">
          <p className="text-zinc-600">Tu carrito está vacío.</p>
          <Link
            href="/"
            className="mt-4 inline-block text-sky-700 underline"
          >
            Ir al catálogo
          </Link>
        </div>
      ) : (
        <form onSubmit={handleConfirm} className="mt-8 space-y-8">
          <ul className="divide-y divide-zinc-200 rounded-xl border border-zinc-200 bg-white">
            {items.map((item) => (
              <li
                key={item.idProducto}
                className="flex flex-wrap items-center justify-between gap-4 px-4 py-4"
              >
                <div>
                  <p className="font-medium text-zinc-900">{item.nombre}</p>
                  <p className="text-sm text-zinc-500">
                    C${item.precio.toFixed(2)} c/u
                  </p>
                </div>
                <div className="flex items-center gap-3">
                  <input
                    type="number"
                    min={1}
                    max={item.existencias}
                    value={item.cantidad}
                    onChange={(e) =>
                      updateQuantity(
                        item.idProducto,
                        parseInt(e.target.value, 10) || 0,
                      )
                    }
                    className="w-16 rounded border border-zinc-300 px-2 py-1 text-center text-sm"
                  />
                  <span className="min-w-[5rem] text-right font-medium">
                    C${(item.precio * item.cantidad).toFixed(2)}
                  </span>
                  <button
                    type="button"
                    onClick={() => removeItem(item.idProducto)}
                    className="text-sm text-red-600 hover:underline"
                  >
                    Quitar
                  </button>
                </div>
              </li>
            ))}
          </ul>

          <div className="rounded-xl border border-zinc-200 bg-white p-6">
            <p className="flex justify-between text-lg font-semibold">
              <span>Total</span>
              <span>C${subtotal.toFixed(2)}</span>
            </p>

            <label
              htmlFor="direccion"
              className="mt-4 block text-sm font-medium text-zinc-700"
            >
              Dirección de entrega
            </label>
            <textarea
              id="direccion"
              required
              rows={3}
              value={direccion}
              onChange={(e) => setDireccion(e.target.value)}
              placeholder="Barrio, referencias, municipio…"
              className="mt-1 w-full rounded-lg border border-zinc-300 px-3 py-2 text-sm outline-none ring-sky-500 focus:ring-2"
            />

            {error && (
              <p className="mt-3 text-sm text-red-600" role="alert">
                {error}
              </p>
            )}

            <button
              type="submit"
              disabled={submitting}
              className="mt-4 w-full rounded-lg bg-emerald-600 py-3 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-60"
            >
              {submitting ? "Procesando…" : "Confirmar pedido"}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
