namespace NicaLogistics.Application.UseCases.Encomiendas;

public interface IEncomiendaService
{
    Task<IReadOnlyList<EncomiendaDto>> GetPropiasAsync(int idCliente, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EncomiendaDto>> GetTodasAsync(CancellationToken cancellationToken = default);
    Task<EncomiendaDto> ActualizarEstadoAsync(int idEncomienda, ActualizarEstadoEncomiendaRequest request, CancellationToken cancellationToken = default);
}
