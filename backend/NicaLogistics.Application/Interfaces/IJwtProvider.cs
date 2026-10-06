using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.Interfaces;

public interface IJwtProvider
{
    string Generate(Usuario usuario);
}
