namespace NicaLogistics.Domain.Entities;

public class Pedido
{
    private readonly List<DetallePedido> _detalles = new();

    public int IdPedido { get; private set; }
    public DateTime FechaPedido { get; private set; }
    public string Estado { get; private set; } = "Pendiente";
    public decimal Total { get; private set; }
    public int IdCliente { get; private set; }
    public Cliente? Cliente { get; private set; }

    public IReadOnlyCollection<DetallePedido> Detalles => _detalles.AsReadOnly();
    public Envio? Envio { get; private set; }

    // Constructor protegido para EF Core
    protected Pedido() { }

    public Pedido(int idCliente)
    {
        if (idCliente <= 0)
            throw new ArgumentException("El ID del cliente debe ser válido.", nameof(idCliente));

        IdCliente = idCliente;
        FechaPedido = DateTime.UtcNow;
        Estado = "Pendiente";
        Total = 0m;
    }

    public Pedido(int idPedido, int idCliente, DateTime fechaPedido, string estado, decimal total)
    {
        IdPedido = idPedido;
        IdCliente = idCliente;
        FechaPedido = fechaPedido;
        Estado = estado;
        Total = total;
    }

    public void AgregarDetalle(int idProducto, int cantidad, decimal precioUnitario)
    {
        if (Estado != "Pendiente")
            throw new InvalidOperationException("Solo se pueden agregar productos a un pedido en estado Pendiente.");

        var detalle = new DetallePedido(idProducto, cantidad, precioUnitario);
        _detalles.Add(detalle);
        RecalcularTotal();
    }

    public void AsignarEnvio(Envio envio)
    {
        Envio = envio ?? throw new ArgumentNullException(nameof(envio));
    }

    public void ActualizarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
            throw new ArgumentException("El estado no puede estar vacío.", nameof(nuevoEstado));

        Estado = nuevoEstado.Trim();
    }

    public void Cancelar()
    {
        if (Estado is "Enviado" or "Entregado")
            throw new InvalidOperationException("No se puede cancelar un pedido que ya ha sido enviado o entregado.");

        Estado = "Cancelado";
        if (Envio != null)
        {
            Envio.ActualizarEstado("Cancelado");
        }
    }

    private void RecalcularTotal()
    {
        Total = _detalles.Sum(d => d.Subtotal);
    }
}
