namespace NicaLogistics.Domain.Entities;

public class Envio
{
    public int IdEnvio { get; private set; }
    public int IdPedido { get; private set; }
    public Pedido? Pedido { get; private set; }
    public string DireccionDestino { get; private set; } = string.Empty;
    public string EstadoEnvio { get; private set; } = "EnPreparacion";
    public DateTime? FechaEstimada { get; private set; }
    public string GuiaRastreo { get; private set; } = string.Empty;

    // Constructor protegido para EF Core
    protected Envio() { }

    public Envio(string direccionDestino, string guiaRastreo, DateTime? fechaEstimada = null, string estadoEnvio = "EnPreparacion")
    {
        if (string.IsNullOrWhiteSpace(direccionDestino))
            throw new ArgumentException("La dirección de destino no puede estar vacía.", nameof(direccionDestino));

        if (string.IsNullOrWhiteSpace(guiaRastreo))
            throw new ArgumentException("La guía de rastreo no puede estar vacía.", nameof(guiaRastreo));

        DireccionDestino = direccionDestino.Trim();
        GuiaRastreo = guiaRastreo.Trim().ToUpperInvariant();
        EstadoEnvio = string.IsNullOrWhiteSpace(estadoEnvio) ? "EnPreparacion" : estadoEnvio.Trim();
        FechaEstimada = fechaEstimada;
    }

    public Envio(int idEnvio, int idPedido, string direccionDestino, string guiaRastreo, DateTime? fechaEstimada = null, string estadoEnvio = "EnPreparacion")
        : this(direccionDestino, guiaRastreo, fechaEstimada, estadoEnvio)
    {
        IdEnvio = idEnvio;
        IdPedido = idPedido;
    }

    public void AsociarPedido(int idPedido)
    {
        if (idPedido <= 0)
            throw new ArgumentException("El ID del pedido debe ser válido.", nameof(idPedido));

        IdPedido = idPedido;
    }

    public void ActualizarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
            throw new ArgumentException("El estado de envío no puede estar vacío.", nameof(nuevoEstado));

        EstadoEnvio = nuevoEstado.Trim();
    }

    public void ActualizarFechaEstimada(DateTime fechaEstimada)
    {
        FechaEstimada = fechaEstimada;
    }

    public void ActualizarDireccion(string nuevaDireccion)
    {
        if (string.IsNullOrWhiteSpace(nuevaDireccion))
            throw new ArgumentException("La dirección no puede estar vacía.", nameof(nuevaDireccion));

        DireccionDestino = nuevaDireccion.Trim();
    }
}
