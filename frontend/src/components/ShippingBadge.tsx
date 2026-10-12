import type { EstadoEnvio } from "@/types";

const STYLES: Record<string, string> = {
  EnPreparacion: "bg-amber-100 text-amber-900 ring-amber-200",
  EnTransito: "bg-sky-100 text-sky-900 ring-sky-200",
  Entregado: "bg-emerald-100 text-emerald-900 ring-emerald-200",
};

function labelFor(estado: string): string {
  switch (estado) {
    case "EnPreparacion":
      return "En preparación";
    case "EnTransito":
      return "En tránsito";
    case "Entregado":
      return "Entregado";
    default:
      return estado;
  }
}

export function ShippingBadge({ estado }: { estado: string }) {
  const style =
    STYLES[estado] ?? "bg-zinc-100 text-zinc-800 ring-zinc-200";

  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-inset ${style}`}
    >
      {labelFor(estado as EstadoEnvio)}
    </span>
  );
}
