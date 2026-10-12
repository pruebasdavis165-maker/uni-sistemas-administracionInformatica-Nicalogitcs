export interface Usuario {
  idUsuario: number;
  nombre: string;
  correo: string;
  rol: string;
}

export interface Producto {
  idProducto: number;
  nombre: string;
  descripcion: string;
  precio: number;
  existencias: number;
  estado: boolean;
  idCategoria: number;
  nombreCategoria: string;
}

export interface DetallePedido {
  idDetalle: number;
  idProducto: number;
  nombreProducto: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface Envio {
  idEnvio: number;
  idPedido: number;
  direccionDestino: string;
  estadoEnvio: string;
  fechaEstimada: string | null;
  guiaRastreo: string;
}

export interface Pedido {
  idPedido: number;
  fechaPedido: string;
  estado: string;
  total: number;
  idCliente: number;
  detalles: DetallePedido[];
  envio: Envio | null;
}

export interface LoginRequest {
  correo: string;
  contrasena: string;
}

export interface RegisterRequest {
  nombre: string;
  correo: string;
  contrasena: string;
}

export interface LoginResponse {
  token: string;
}

export interface ItemPedidoRequest {
  idProducto: number;
  cantidad: number;
}

export interface CrearPedidoRequest {
  items: ItemPedidoRequest[];
  direccionDestino: string;
}

export interface UpdateInventarioRequest {
  cantidad: number;
}

export interface ActualizarEstadoEnvioRequest {
  nuevoEstado: string;
  fechaEstimada?: string | null;
}

export interface CartItem {
  idProducto: number;
  nombre: string;
  precio: number;
  cantidad: number;
  existencias: number;
}

export const ESTADOS_ENVIO = [
  "EnPreparacion",
  "EnTransito",
  "Entregado",
] as const;

export type EstadoEnvio = (typeof ESTADOS_ENVIO)[number];

export interface Pago {
  idPago: number;
  idPedido: number;
  fechaPago: string;
  monto: number;
  estado: "Pendiente" | "Completado" | "Cancelado" | "Reembolsado" | string;
  metodoPago: string;
  fechaReembolso: string | null;
}

export interface Encomienda {
  idEncomienda: number;
  idPedido: number;
  origen: string;
  destino: string;
  estado: "Registrada" | "EnTransito" | "Entregada" | "Cancelada" | string;
  fechaRegistro: string;
  guiaRastreo?: string | null;
}

export interface RegistrarPagoRequest {
  idPedido: number;
  metodoPago: string;
}

export interface RegistrarPagoResponse {
  pago: Pago;
  encomienda: Encomienda;
}

export interface ActualizarEstadoEncomiendaRequest {
  nuevoEstado: string;
}

export const ESTADOS_ENCOMIENDA = [
  "Registrada",
  "EnTransito",
  "Entregada",
] as const;

export type EstadoEncomienda = (typeof ESTADOS_ENCOMIENDA)[number];

