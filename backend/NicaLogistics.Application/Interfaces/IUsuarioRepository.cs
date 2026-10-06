using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
