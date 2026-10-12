import type {
  ActualizarEstadoEnvioRequest,
  CrearPedidoRequest,
  Pedido,
} from "@/types";
import { apiClient } from "./client";

export async function crearPedido(request: CrearPedidoRequest): Promise<Pedido> {
  return apiClient<Pedido>("/Pedidos", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export async function getMisPedidos(): Promise<Pedido[]> {
  return apiClient<Pedido[]>("/Pedidos/mis-pedidos");
}

export async function getAllPedidos(): Promise<Pedido[]> {
  return apiClient<Pedido[]>("/Pedidos");
}

export async function getPedidoById(id: number): Promise<Pedido> {
  return apiClient<Pedido>(`/Pedidos/${id}`);
}

export async function updateEstadoEnvio(
  idPedido: number,
  request: ActualizarEstadoEnvioRequest,
): Promise<Pedido> {
  return apiClient<Pedido>(`/Pedidos/${idPedido}/estado-envio`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
