namespace NicaLogistics.Application.UseCases.Orders;

public interface IPedidoService
{
    Task<PedidoDto> CrearPedidoAsync(int idCliente, CrearPedidoRequest request, CancellationToken cancellationToken = default);
    Task<PedidoDto> GetByIdAsync(int idPedido, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PedidoDto>> GetMisPedidosAsync(int idCliente, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PedidoDto>> GetTodosPedidosAsync(CancellationToken cancellationToken = default);
    Task<PedidoDto> ActualizarEstadoEnvioAsync(int idPedido, ActualizarEstadoEnvioRequest request, CancellationToken cancellationToken = default);
}
