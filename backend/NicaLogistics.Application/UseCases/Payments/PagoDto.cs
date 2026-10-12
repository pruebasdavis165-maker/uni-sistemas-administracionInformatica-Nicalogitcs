using NicaLogistics.Application.UseCases.Encomiendas;

namespace NicaLogistics.Application.UseCases.Payments;

public record RegistrarPagoRequest(int IdPedido, string MetodoPago);

public record PagoDto(
    int IdPago,
    int IdPedido,
    DateTime FechaPago,
    decimal Monto,
    string Estado,
    string MetodoPago,
    DateTime? FechaReembolso
);

/// <summary>Resultado de registrar un pago: incluye la encomienda generada automáticamente.</summary>
public record RegistrarPagoResponse(PagoDto Pago, EncomiendaDto Encomienda);
