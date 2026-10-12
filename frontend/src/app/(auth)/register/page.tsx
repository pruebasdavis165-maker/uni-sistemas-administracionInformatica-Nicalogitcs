"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { ApiError } from "@/services/api/client";

export default function RegisterPage() {
  const { register, isAuthenticated, isLoading } = useAuth();
  const router = useRouter();
  const [nombre, setNombre] = useState("");
  const [correo, setCorreo] = useState("");
  const [contrasena, setContrasena] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  if (!isLoading && isAuthenticated) {
    router.replace("/");
    return null;
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await register({ nombre, correo, contrasena });
      router.push("/");
    } catch (err) {
      setError(
        err instanceof ApiError ? err.message : "Error al registrarse.",
      );
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="flex flex-1 items-center justify-center px-4 py-12">
      <div className="w-full max-w-md rounded-2xl border border-zinc-200 bg-white p-8 shadow-sm">
        <h1 className="text-2xl font-bold text-zinc-900">Crear cuenta</h1>
        <p className="mt-1 text-sm text-zinc-600">
          Regístrate para realizar pedidos en NicaLogistics.
        </p>

        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          <div>
            <label htmlFor="nombre" className="block text-sm font-medium text-zinc-700">
              Nombre completo
            </label>
            <input
              id="nombre"
              type="text"
              required
              autoComplete="name"
              value={nombre}
              onChange={(e) => setNombre(e.target.value)}
              className="mt-1 w-full rounded-lg border border-zinc-300 px-3 py-2 text-sm outline-none ring-sky-500 focus:ring-2"
            />
          </div>
          <div>
            <label htmlFor="correo" className="block text-sm font-medium text-zinc-700">
              Correo
            </label>
            <input
              id="correo"
              type="email"
              required
              autoComplete="email"
              value={correo}
              onChange={(e) => setCorreo(e.target.value)}
              className="mt-1 w-full rounded-lg border border-zinc-300 px-3 py-2 text-sm outline-none ring-sky-500 focus:ring-2"
            />
          </div>
          <div>
            <label
              htmlFor="contrasena"
              className="block text-sm font-medium text-zinc-700"
            >
              Contraseña
            </label>
            <input
              id="contrasena"
              type="password"
              required
              minLength={6}
              autoComplete="new-password"
              value={contrasena}
              onChange={(e) => setContrasena(e.target.value)}
              className="mt-1 w-full rounded-lg border border-zinc-300 px-3 py-2 text-sm outline-none ring-sky-500 focus:ring-2"
            />
          </div>

          {error && (
            <p className="text-sm text-red-600" role="alert">
              {error}
            </p>
          )}

          <button
            type="submit"
            disabled={submitting}
            className="w-full rounded-lg bg-sky-600 py-2.5 text-sm font-semibold text-white hover:bg-sky-700 disabled:opacity-60"
          >
            {submitting ? "Creando cuenta…" : "Registrarse"}
          </button>
        </form>

        <p className="mt-6 text-center text-sm text-zinc-600">
          ¿Ya tienes cuenta?{" "}
          <Link href="/login" className="font-medium text-sky-700 hover:underline">
            Inicia sesión
          </Link>
        </p>
      </div>
    </div>
  );
}
