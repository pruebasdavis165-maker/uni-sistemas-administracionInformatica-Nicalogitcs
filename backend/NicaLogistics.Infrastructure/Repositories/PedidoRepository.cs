using Microsoft.EntityFrameworkCore;
using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;
using NicaLogistics.Infrastructure.Persistence;

namespace NicaLogistics.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly ApplicationDbContext _context;

    public PedidoRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Pedido?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Pedidos
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
            .Include(p => p.Envio)
            .Include(p => p.Cliente)
            .FirstOrDefaultAsync(p => p.IdPedido == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Pedido>> GetByClienteIdAsync(int idCliente, CancellationToken cancellationToken = default)
    {
        return await _context.Pedidos
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
            .Include(p => p.Envio)
            .Where(p => p.IdCliente == idCliente)
            .OrderByDescending(p => p.FechaPedido)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        if (pedido is null)
            throw new ArgumentNullException(nameof(pedido));

        await _context.Pedidos.AddAsync(pedido, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        if (pedido is null)
            throw new ArgumentNullException(nameof(pedido));

        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
