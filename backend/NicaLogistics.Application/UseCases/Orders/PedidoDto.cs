namespace NicaLogistics.Application.UseCases.Orders;

public record DetallePedidoDto(
    int IdDetalle,
    int IdProducto,
    string NombreProducto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal
);

public record EnvioDto(
    int IdEnvio,
    int IdPedido,
    string DireccionDestino,
    string EstadoEnvio,
    DateTime? FechaEstimada,
    string GuiaRastreo
);

public record PedidoDto(
    int IdPedido,
    DateTime FechaPedido,
    string Estado,
    decimal Total,
    int IdCliente,
    IReadOnlyList<DetallePedidoDto> Detalles,
    EnvioDto? Envio
);
