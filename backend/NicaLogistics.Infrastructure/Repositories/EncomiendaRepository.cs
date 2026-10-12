using Microsoft.EntityFrameworkCore;
using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;
using NicaLogistics.Infrastructure.Persistence;

namespace NicaLogistics.Infrastructure.Repositories;

public class EncomiendaRepository : IEncomiendaRepository
{
    private readonly ApplicationDbContext _context;

    public EncomiendaRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    private IQueryable<Encomienda> Query() =>
        _context.Encomiendas
            .Include(e => e.Pedido)
                .ThenInclude(p => p!.Envio);

    public async Task<Encomienda?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(e => e.IdEncomienda == id, cancellationToken);
    }

    public async Task<Encomienda?> GetByPedidoIdAsync(int idPedido, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(e => e.IdPedido == idPedido, cancellationToken);
    }

    public async Task<IReadOnlyList<Encomienda>> GetByClienteIdAsync(int idCliente, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(e => e.Pedido!.IdCliente == idCliente)
            .OrderByDescending(e => e.FechaRegistro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Encomienda>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .OrderByDescending(e => e.FechaRegistro)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Encomienda encomienda, CancellationToken cancellationToken = default)
    {
        if (encomienda is null)
            throw new ArgumentNullException(nameof(encomienda));

        await _context.Encomiendas.AddAsync(encomienda, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Encomienda encomienda, CancellationToken cancellationToken = default)
    {
        if (encomienda is null)
            throw new ArgumentNullException(nameof(encomienda));

        _context.Encomiendas.Update(encomienda);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
