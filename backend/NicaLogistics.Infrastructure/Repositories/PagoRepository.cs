using Microsoft.EntityFrameworkCore;
using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;
using NicaLogistics.Infrastructure.Persistence;

namespace NicaLogistics.Infrastructure.Repositories;

public class PagoRepository : IPagoRepository
{
    private readonly ApplicationDbContext _context;

    public PagoRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Pago?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Pagos
            .Include(p => p.Pedido)
            .FirstOrDefaultAsync(p => p.IdPago == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Pago>> GetByPedidoIdAsync(int idPedido, CancellationToken cancellationToken = default)
    {
        return await _context.Pagos
            .Where(p => p.IdPedido == idPedido)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pago>> GetByClienteIdAsync(int idCliente, CancellationToken cancellationToken = default)
    {
        return await _context.Pagos
            .Where(p => p.Pedido!.IdCliente == idCliente)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pago>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pagos
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Pago pago, CancellationToken cancellationToken = default)
    {
        if (pago is null)
            throw new ArgumentNullException(nameof(pago));

        await _context.Pagos.AddAsync(pago, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Pago pago, CancellationToken cancellationToken = default)
    {
        if (pago is null)
            throw new ArgumentNullException(nameof(pago));

        _context.Pagos.Update(pago);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
