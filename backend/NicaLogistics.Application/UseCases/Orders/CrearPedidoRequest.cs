namespace NicaLogistics.Application.UseCases.Orders;

public record ItemPedidoRequest(int IdProducto, int Cantidad);

public record CrearPedidoRequest(
    List<ItemPedidoRequest> Items,
    string DireccionDestino
);
