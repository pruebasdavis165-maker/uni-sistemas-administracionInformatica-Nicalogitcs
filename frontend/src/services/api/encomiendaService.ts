import type {
  ActualizarEstadoEncomiendaRequest,
  Encomienda,
} from "@/types";
import { apiClient } from "./client";

export async function getEncomiendasPropias(): Promise<Encomienda[]> {
  return apiClient<Encomienda[]>("/Encomiendas/propia");
}

export async function getAllEncomiendas(): Promise<Encomienda[]> {
  return apiClient<Encomienda[]>("/Encomiendas");
}

export async function actualizarEstadoEncomienda(
  idEncomienda: number,
  request: ActualizarEstadoEncomiendaRequest,
): Promise<Encomienda> {
  return apiClient<Encomienda>(`/Encomiendas/${idEncomienda}/estado`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
