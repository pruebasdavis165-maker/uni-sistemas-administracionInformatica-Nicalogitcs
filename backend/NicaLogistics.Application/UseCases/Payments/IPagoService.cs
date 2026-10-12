namespace NicaLogistics.Application.UseCases.Payments;

public interface IPagoService
{
    Task<RegistrarPagoResponse> RegistrarPagoAsync(int idUsuario, bool esAdmin, RegistrarPagoRequest request, CancellationToken cancellationToken = default);
    Task<PagoDto> GetEstadoAsync(int idPago, int idUsuario, bool esAdmin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PagoDto>> GetMisPagosAsync(int idUsuario, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PagoDto>> GetTodosAsync(CancellationToken cancellationToken = default);
    Task<PagoDto> CancelarPagoAsync(int idPago, int idUsuario, bool esAdmin, CancellationToken cancellationToken = default);
    Task<PagoDto> ProcesarReembolsoAsync(int idPago, CancellationToken cancellationToken = default);
}
