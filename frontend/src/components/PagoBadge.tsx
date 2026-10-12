const STYLES: Record<string, string> = {
  Pendiente: "bg-amber-100 text-amber-900 ring-amber-200",
  Completado: "bg-emerald-100 text-emerald-900 ring-emerald-200",
  Cancelado: "bg-zinc-100 text-zinc-700 ring-zinc-200",
  Reembolsado: "bg-purple-100 text-purple-900 ring-purple-200",
};

export function PagoBadge({ estado }: { estado: string }) {
  const style = STYLES[estado] ?? "bg-zinc-100 text-zinc-800 ring-zinc-200";

  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-inset ${style}`}
    >
      💳 {estado}
    </span>
  );
}
