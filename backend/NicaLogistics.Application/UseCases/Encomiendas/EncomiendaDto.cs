namespace NicaLogistics.Application.UseCases.Encomiendas;

public record EncomiendaDto(
    int IdEncomienda,
    int IdPedido,
    string Origen,
    string Destino,
    string Estado,
    DateTime FechaRegistro,
    string? GuiaRastreo
);

public record ActualizarEstadoEncomiendaRequest(string NuevoEstado);
