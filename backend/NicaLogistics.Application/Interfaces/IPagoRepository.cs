using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.Interfaces;

public interface IPagoRepository
{
    Task<Pago?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pago>> GetByPedidoIdAsync(int idPedido, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pago>> GetByClienteIdAsync(int idCliente, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pago>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Pago pago, CancellationToken cancellationToken = default);
    Task UpdateAsync(Pago pago, CancellationToken cancellationToken = default);
}
