using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.Interfaces;

public interface IPedidoRepository
{
    Task<Pedido?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pedido>> GetByClienteIdAsync(int idCliente, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pedido>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task UpdateAsync(Pedido pedido, CancellationToken cancellationToken = default);
}
