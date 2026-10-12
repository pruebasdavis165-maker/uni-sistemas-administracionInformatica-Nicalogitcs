namespace NicaLogistics.Domain.Entities;

public class Pago
{
    public const string EstadoPendiente = "Pendiente";
    public const string EstadoCompletado = "Completado";
    public const string EstadoCancelado = "Cancelado";
    public const string EstadoReembolsado = "Reembolsado";

    public int IdPago { get; private set; }
    public int IdPedido { get; private set; }
    public Pedido? Pedido { get; private set; }
    public DateTime FechaPago { get; private set; }
    public decimal Monto { get; private set; }
    public string Estado { get; private set; } = EstadoPendiente;
    public string MetodoPago { get; private set; } = string.Empty;
    public DateTime? FechaReembolso { get; private set; }

    // Constructor protegido para EF Core
    protected Pago() { }

    private Pago(int idPedido, decimal monto, string metodoPago)
    {
        IdPedido = idPedido;
        Monto = monto;
        MetodoPago = metodoPago;
        FechaPago = DateTime.UtcNow;
        Estado = EstadoPendiente;
    }

    /// <summary>Registra un nuevo pago en estado Pendiente.</summary>
    public static Pago Registrar(int idPedido, decimal monto, string metodoPago)
    {
        if (idPedido <= 0)
            throw new ArgumentException("El ID del pedido debe ser válido.", nameof(idPedido));

        if (monto <= 0)
            throw new ArgumentException("El monto del pago debe ser mayor que cero.", nameof(monto));

        if (string.IsNullOrWhiteSpace(metodoPago))
            throw new ArgumentException("El método de pago es obligatorio.", nameof(metodoPago));

        return new Pago(idPedido, monto, metodoPago.Trim());
    }

    public void Confirmar()
    {
        if (Estado != EstadoPendiente)
            throw new InvalidOperationException($"Solo se puede confirmar un pago Pendiente (estado actual: {Estado}).");

        Estado = EstadoCompletado;
        FechaPago = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        if (Estado != EstadoPendiente)
            throw new InvalidOperationException($"Solo se puede cancelar un pago Pendiente (estado actual: {Estado}).");

        Estado = EstadoCancelado;
    }

    public void ProcesarReembolso()
    {
        if (Estado != EstadoCompletado)
            throw new InvalidOperationException($"Solo se puede reembolsar un pago Completado (estado actual: {Estado}).");

        Estado = EstadoReembolsado;
        FechaReembolso = DateTime.UtcNow;
    }
}
