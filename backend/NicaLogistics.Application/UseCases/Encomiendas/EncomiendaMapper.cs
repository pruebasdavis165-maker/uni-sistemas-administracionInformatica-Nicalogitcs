using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.UseCases.Encomiendas;

public static class EncomiendaMapper
{
    public static EncomiendaDto ToDto(Encomienda e) => new(
        IdEncomienda: e.IdEncomienda,
        IdPedido: e.IdPedido,
        Origen: e.Origen,
        Destino: e.Destino,
        Estado: e.Estado,
        FechaRegistro: e.FechaRegistro,
        GuiaRastreo: e.Pedido?.Envio?.GuiaRastreo
    );
}
