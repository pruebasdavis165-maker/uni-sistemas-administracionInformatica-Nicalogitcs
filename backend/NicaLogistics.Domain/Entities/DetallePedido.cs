namespace NicaLogistics.Domain.Entities;

public class DetallePedido
{
    public int IdDetalle { get; private set; }
    public int IdPedido { get; private set; }
    public Pedido? Pedido { get; private set; }
    public int IdProducto { get; private set; }
    public Producto? Producto { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal { get; private set; }

    // Constructor protegido para EF Core
    protected DetallePedido() { }

    public DetallePedido(int idProducto, int cantidad, decimal precioUnitario)
    {
        if (idProducto <= 0)
            throw new ArgumentException("El ID del producto debe ser válido.", nameof(idProducto));

        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor que cero.", nameof(cantidad));

        if (precioUnitario < 0)
            throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(precioUnitario));

        IdProducto = idProducto;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Subtotal = cantidad * precioUnitario;
    }

    public DetallePedido(int idDetalle, int idPedido, int idProducto, int cantidad, decimal precioUnitario)
        : this(idProducto, cantidad, precioUnitario)
    {
        IdDetalle = idDetalle;
        IdPedido = idPedido;
    }

    public void AsociarPedido(int idPedido)
    {
        if (idPedido <= 0)
            throw new ArgumentException("El ID del pedido debe ser válido.", nameof(idPedido));

        IdPedido = idPedido;
    }
}
