import type { Usuario } from "@/types";

const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

export function parseJwtPayload(token: string): Record<string, unknown> | null {
  try {
    const parts = token.split(".");
    if (parts.length < 2) return null;
    const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const json = atob(base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "="));
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

export function usuarioFromToken(token: string): Usuario | null {
  const payload = parseJwtPayload(token);
  if (!payload) return null;

  const sub = payload.sub ?? payload.nameid;
  const idUsuario =
    typeof sub === "string" ? parseInt(sub, 10) : typeof sub === "number" ? sub : 0;

  const nombre =
    (payload.unique_name as string) ??
    (payload.name as string) ??
    (payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] as string) ??
    "";

  const correo =
    (payload.email as string) ??
    (payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] as string) ??
    "";

  const rol =
    (payload.role as string) ??
    (payload[ROLE_CLAIM] as string) ??
    "Cliente";

  if (!idUsuario) return null;

  return { idUsuario, nombre, correo, rol };
}
