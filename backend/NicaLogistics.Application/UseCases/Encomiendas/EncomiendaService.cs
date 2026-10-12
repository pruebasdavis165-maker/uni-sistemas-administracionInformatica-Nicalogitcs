using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.UseCases.Encomiendas;

public class EncomiendaService : IEncomiendaService
{
    private readonly IEncomiendaRepository _encomiendaRepository;

    public EncomiendaService(IEncomiendaRepository encomiendaRepository)
    {
        _encomiendaRepository = encomiendaRepository ?? throw new ArgumentNullException(nameof(encomiendaRepository));
    }

    public async Task<IReadOnlyList<EncomiendaDto>> GetPropiasAsync(int idCliente, CancellationToken cancellationToken = default)
    {
        var encomiendas = await _encomiendaRepository.GetByClienteIdAsync(idCliente, cancellationToken);
        return encomiendas.Select(EncomiendaMapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<EncomiendaDto>> GetTodasAsync(CancellationToken cancellationToken = default)
    {
        var encomiendas = await _encomiendaRepository.GetAllAsync(cancellationToken);
        return encomiendas.Select(EncomiendaMapper.ToDto).ToList();
    }

    public async Task<EncomiendaDto> ActualizarEstadoAsync(
        int idEncomienda,
        ActualizarEstadoEncomiendaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (idEncomienda <= 0)
            throw new ArgumentException("El ID de la encomienda debe ser válido.", nameof(idEncomienda));

        var encomienda = await _encomiendaRepository.GetByIdAsync(idEncomienda, cancellationToken)
            ?? throw new KeyNotFoundException($"La encomienda con ID {idEncomienda} no fue encontrada.");

        var estadoAnterior = encomienda.Estado;
        encomienda.ActualizarEstado(request.NuevoEstado); // valida estado y estados finales
        var nuevoEstado = encomienda.Estado;

        if (nuevoEstado == Encomienda.EstadoCancelada)
            throw new InvalidOperationException("La cancelación de una encomienda se realiza mediante el reembolso del pago.");

        if (Rango(nuevoEstado) < Rango(estadoAnterior))
            throw new InvalidOperationException(
                $"No se puede retroceder una encomienda de {estadoAnterior} a {nuevoEstado}.");

        // Mantener sincronizados el envío y el pedido con la encomienda.
        var pedido = encomienda.Pedido;
        if (pedido is not null)
        {
            switch (nuevoEstado)
            {
                case Encomienda.EstadoEnTransito:
                    pedido.Envio?.ActualizarEstado("EnTransito");
                    pedido.ActualizarEstado("Enviado");
                    break;
                case Encomienda.EstadoEntregada:
                    pedido.Envio?.ActualizarEstado("Entregado");
                    pedido.ActualizarEstado("Entregado");
                    break;
            }
        }

        await _encomiendaRepository.UpdateAsync(encomienda, cancellationToken);
        return EncomiendaMapper.ToDto(encomienda);
    }

    private static int Rango(string estado) => estado switch
    {
        Encomienda.EstadoRegistrada => 0,
        Encomienda.EstadoEnTransito => 1,
        Encomienda.EstadoEntregada => 2,
        _ => 3
    };
}
