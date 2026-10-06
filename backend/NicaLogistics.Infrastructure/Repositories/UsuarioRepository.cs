using Microsoft.EntityFrameworkCore;
using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;
using NicaLogistics.Infrastructure.Persistence;

namespace NicaLogistics.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Usuario?> GetByCorreoAsync(string correo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(correo))
            return null;

        var normalizedEmail = correo.Trim().ToLowerInvariant();

        return await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Correo == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        if (usuario is null)
            throw new ArgumentNullException(nameof(usuario));

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
