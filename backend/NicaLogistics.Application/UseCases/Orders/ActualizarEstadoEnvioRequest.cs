namespace NicaLogistics.Application.UseCases.Orders;

public record ActualizarEstadoEnvioRequest(
    string NuevoEstado,
    DateTime? FechaEstimada = null
);
