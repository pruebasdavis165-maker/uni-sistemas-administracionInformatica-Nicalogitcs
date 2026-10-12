"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { useCart } from "@/context/CartContext";

function NavLink({
  href,
  children,
}: {
  href: string;
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const active = pathname === href;
  return (
    <Link
      href={href}
      className={`rounded-md px-3 py-2 text-sm font-medium transition ${
        active
          ? "bg-sky-100 text-sky-900"
          : "text-zinc-700 hover:bg-zinc-100 hover:text-zinc-900"
      }`}
    >
      {children}
    </Link>
  );
}

export function Navbar() {
  const { isAuthenticated, isAdmin, user, logout, isLoading } = useAuth();
  const { itemCount } = useCart();

  return (
    <header className="sticky top-0 z-50 border-b border-zinc-200 bg-white/95 backdrop-blur">
      <div className="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-3 px-4 py-3">
        <Link href="/" className="text-lg font-bold tracking-tight text-sky-800">
          NicaLogistics
        </Link>

        <nav className="flex flex-wrap items-center gap-1">
          <NavLink href="/">Catálogo</NavLink>
          {isAuthenticated && (
            <>
              <NavLink href="/mis-pedidos">Mis pedidos</NavLink>
              <NavLink href="/checkout">
                Carrito
                {itemCount > 0 && (
                  <span className="ml-1.5 inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-sky-600 px-1 text-xs text-white">
                    {itemCount}
                  </span>
                )}
              </NavLink>
            </>
          )}
          {isAdmin && (
            <>
              <NavLink href="/admin/inventario">Inventario</NavLink>
              <NavLink href="/admin/envios">Envíos</NavLink>
              <NavLink href="/admin/encomiendas">Encomiendas</NavLink>
              <NavLink href="/admin/pagos">Pagos</NavLink>
            </>
          )}
        </nav>

        <div className="flex items-center gap-2">
          {isLoading ? (
            <span className="text-sm text-zinc-400">…</span>
          ) : isAuthenticated && user ? (
            <>
              <span className="hidden text-sm text-zinc-600 sm:inline">
                {user.nombre}
              </span>
              <button
                type="button"
                onClick={logout}
                className="rounded-lg border border-zinc-300 px-3 py-1.5 text-sm font-medium text-zinc-800 hover:bg-zinc-50"
              >
                Salir
              </button>
            </>
          ) : (
            <>
              <Link
                href="/login"
                className="rounded-lg px-3 py-1.5 text-sm font-medium text-zinc-700 hover:bg-zinc-100"
              >
                Ingresar
              </Link>
              <Link
                href="/register"
                className="rounded-lg bg-sky-600 px-3 py-1.5 text-sm font-medium text-white hover:bg-sky-700"
              >
                Registrarse
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
