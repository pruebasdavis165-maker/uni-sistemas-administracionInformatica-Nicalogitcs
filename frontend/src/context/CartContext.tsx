"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import type { CartItem, Producto } from "@/types";

const CART_KEY = "nicalogistics_cart";

interface CartContextValue {
  items: CartItem[];
  itemCount: number;
  subtotal: number;
  addItem: (producto: Producto, cantidad?: number) => void;
  removeItem: (idProducto: number) => void;
  updateQuantity: (idProducto: number, cantidad: number) => void;
  clearCart: () => void;
}

const CartContext = createContext<CartContextValue | null>(null);

function loadCart(): CartItem[] {
  if (typeof window === "undefined") return [];
  const raw = localStorage.getItem(CART_KEY);
  if (!raw) return [];
  try {
    return JSON.parse(raw) as CartItem[];
  } catch {
    return [];
  }
}

export function CartProvider({ children }: { children: React.ReactNode }) {
  const [items, setItems] = useState<CartItem[]>([]);
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    setItems(loadCart());
    setHydrated(true);
  }, []);

  useEffect(() => {
    if (!hydrated) return;
    localStorage.setItem(CART_KEY, JSON.stringify(items));
  }, [items, hydrated]);

  const addItem = useCallback((producto: Producto, cantidad = 1) => {
    if (!producto.estado || producto.existencias <= 0) return;

    setItems((prev) => {
      const existing = prev.find((i) => i.idProducto === producto.idProducto);
      if (existing) {
        const nextQty = Math.min(
          existing.cantidad + cantidad,
          producto.existencias,
        );
        return prev.map((i) =>
          i.idProducto === producto.idProducto
            ? { ...i, cantidad: nextQty, existencias: producto.existencias }
            : i,
        );
      }
      return [
        ...prev,
        {
          idProducto: producto.idProducto,
          nombre: producto.nombre,
          precio: producto.precio,
          cantidad: Math.min(cantidad, producto.existencias),
          existencias: producto.existencias,
        },
      ];
    });
  }, []);

  const removeItem = useCallback((idProducto: number) => {
    setItems((prev) => prev.filter((i) => i.idProducto !== idProducto));
  }, []);

  const updateQuantity = useCallback((idProducto: number, cantidad: number) => {
    if (cantidad <= 0) {
      setItems((prev) => prev.filter((i) => i.idProducto !== idProducto));
      return;
    }
    setItems((prev) =>
      prev.map((i) =>
        i.idProducto === idProducto
          ? {
              ...i,
              cantidad: Math.min(cantidad, i.existencias),
            }
          : i,
      ),
    );
  }, []);

  const clearCart = useCallback(() => setItems([]), []);

  const itemCount = useMemo(
    () => items.reduce((sum, i) => sum + i.cantidad, 0),
    [items],
  );

  const subtotal = useMemo(
    () => items.reduce((sum, i) => sum + i.precio * i.cantidad, 0),
    [items],
  );

  const value = useMemo<CartContextValue>(
    () => ({
      items,
      itemCount,
      subtotal,
      addItem,
      removeItem,
      updateQuantity,
      clearCart,
    }),
    [
      items,
      itemCount,
      subtotal,
      addItem,
      removeItem,
      updateQuantity,
      clearCart,
    ],
  );

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart(): CartContextValue {
  const ctx = useContext(CartContext);
  if (!ctx) {
    throw new Error("useCart debe usarse dentro de CartProvider");
  }
  return ctx;
}
