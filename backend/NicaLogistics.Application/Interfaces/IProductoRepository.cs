using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.Interfaces;

public interface IProductoRepository
{
    Task<IReadOnlyList<Producto>> GetActivosAsync(CancellationToken cancellationToken = default);
    Task<Producto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Producto producto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Producto producto, CancellationToken cancellationToken = default);
}
