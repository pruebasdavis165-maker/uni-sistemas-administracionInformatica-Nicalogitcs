namespace NicaLogistics.Domain.Entities;

public class Encomienda
{
    public const string EstadoRegistrada = "Registrada";
    public const string EstadoEnTransito = "EnTransito";
    public const string EstadoEntregada = "Entregada";
    public const string EstadoCancelada = "Cancelada";

    private static readonly string[] EstadosValidos =
    {
        EstadoRegistrada, EstadoEnTransito, EstadoEntregada, EstadoCancelada
    };

    public int IdEncomienda { get; private set; }
    public int IdPedido { get; private set; }
    public Pedido? Pedido { get; private set; }
    public string Origen { get; private set; } = string.Empty;
    public string Destino { get; private set; } = string.Empty;
    public string Estado { get; private set; } = EstadoRegistrada;
    public DateTime FechaRegistro { get; private set; }

    // Constructor protegido para EF Core
    protected Encomienda() { }

    public Encomienda(int idPedido, string origen, string destino)
    {
        if (idPedido <= 0)
            throw new ArgumentException("El ID del pedido debe ser válido.", nameof(idPedido));

        if (string.IsNullOrWhiteSpace(origen))
            throw new ArgumentException("El origen no puede estar vacío.", nameof(origen));

        if (string.IsNullOrWhiteSpace(destino))
            throw new ArgumentException("El destino no puede estar vacío.", nameof(destino));

        IdPedido = idPedido;
        Origen = origen.Trim();
        Destino = destino.Trim();
        Estado = EstadoRegistrada;
        FechaRegistro = DateTime.UtcNow;
    }

    public void ActualizarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
            throw new ArgumentException("El estado no puede estar vacío.", nameof(nuevoEstado));

        var estado = EstadosValidos.FirstOrDefault(
            e => e.Equals(nuevoEstado.Trim(), StringComparison.OrdinalIgnoreCase));

        if (estado is null)
            throw new ArgumentException(
                $"Estado inválido '{nuevoEstado}'. Valores permitidos: {string.Join(", ", EstadosValidos)}.",
                nameof(nuevoEstado));

        if (Estado is EstadoEntregada or EstadoCancelada && estado != Estado)
            throw new InvalidOperationException($"Una encomienda {Estado} no puede cambiar de estado.");

        Estado = estado;
    }
}
