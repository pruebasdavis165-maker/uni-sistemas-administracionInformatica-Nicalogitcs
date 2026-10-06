using Microsoft.EntityFrameworkCore;
using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;
using NicaLogistics.Infrastructure.Persistence;

namespace NicaLogistics.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;

    public ProductoRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<Producto>> GetActivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Estado)
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<Producto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.IdProducto == id, cancellationToken);
    }

    public async Task AddAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        if (producto is null)
            throw new ArgumentNullException(nameof(producto));

        await _context.Productos.AddAsync(producto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        if (producto is null)
            throw new ArgumentNullException(nameof(producto));

        _context.Productos.Update(producto);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
