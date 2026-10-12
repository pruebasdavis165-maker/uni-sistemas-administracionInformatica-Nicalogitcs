import type { Producto, UpdateInventarioRequest } from "@/types";
import { apiClient } from "./client";

export async function getCatalog(): Promise<Producto[]> {
  return apiClient<Producto[]>("/Catalog");
}

export async function updateInventario(
  idProducto: number,
  request: UpdateInventarioRequest,
): Promise<Producto> {
  return apiClient<Producto>(`/Catalog/${idProducto}/inventario`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
