import type {
  Pago,
  RegistrarPagoRequest,
  RegistrarPagoResponse,
} from "@/types";
import { apiClient } from "./client";

export async function registrarPago(
  request: RegistrarPagoRequest,
): Promise<RegistrarPagoResponse> {
  return apiClient<RegistrarPagoResponse>("/Pagos", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export async function getEstadoPago(idPago: number): Promise<Pago> {
  return apiClient<Pago>(`/Pagos/${idPago}/estado`);
}

export async function getMisPagos(): Promise<Pago[]> {
  return apiClient<Pago[]>("/Pagos/mis-pagos");
}

export async function getAllPagos(): Promise<Pago[]> {
  return apiClient<Pago[]>("/Pagos");
}

export async function cancelarPago(idPago: number): Promise<Pago> {
  return apiClient<Pago>(`/Pagos/${idPago}/cancelar`, {
    method: "POST",
  });
}

export async function reembolsarPago(idPago: number): Promise<Pago> {
  return apiClient<Pago>(`/Pagos/${idPago}/reembolso`, {
    method: "POST",
  });
}
