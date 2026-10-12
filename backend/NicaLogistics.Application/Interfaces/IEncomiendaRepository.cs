using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.Interfaces;

public interface IEncomiendaRepository
{
    Task<Encomienda?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Encomienda?> GetByPedidoIdAsync(int idPedido, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Encomienda>> GetByClienteIdAsync(int idCliente, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Encomienda>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Encomienda encomienda, CancellationToken cancellationToken = default);
    Task UpdateAsync(Encomienda encomienda, CancellationToken cancellationToken = default);
}
