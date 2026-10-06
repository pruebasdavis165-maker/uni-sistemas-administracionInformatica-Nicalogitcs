namespace NicaLogistics.Domain.Entities;

public class Producto
{
    public int IdProducto { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Precio { get; private set; }
    public int Existencias { get; private set; }
    public bool Estado { get; private set; }
    public int IdCategoria { get; private set; }
    public Categoria? Categoria { get; private set; }

    // Constructor protegido para EF Core
    protected Producto() { }

    public Producto(
        string nombre,
        string descripcion,
        decimal precio,
        int existencias,
        int idCategoria,
        bool estado = true)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(nombre));

        if (precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.", nameof(precio));

        if (existencias < 0)
            throw new ArgumentException("Las existencias iniciales no pueden ser negativas.", nameof(existencias));

        if (idCategoria <= 0)
            throw new ArgumentException("Debe especificarse una categoría válida.", nameof(idCategoria));

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
        Precio = precio;
        Existencias = existencias;
        IdCategoria = idCategoria;
        Estado = estado;
    }

    public Producto(
        int idProducto,
        string nombre,
        string descripcion,
        decimal precio,
        int existencias,
        int idCategoria,
        bool estado = true)
        : this(nombre, descripcion, precio, existencias, idCategoria, estado)
    {
        IdProducto = idProducto;
    }

    public void ActualizarExistencias(int cantidad)
    {
        if (cantidad < 0)
            throw new ArgumentException("Las existencias no pueden ser un valor negativo.", nameof(cantidad));

        Existencias = cantidad;
    }

    public void IncrementarExistencias(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a incrementar debe ser mayor que cero.", nameof(cantidad));

        Existencias += cantidad;
    }

    public void DisminuirExistencias(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a disminuir debe ser mayor que cero.", nameof(cantidad));

        if (Existencias - cantidad < 0)
            throw new InvalidOperationException("No hay suficientes existencias para completar la operación.");

        Existencias -= cantidad;
    }

    public bool ConsultarDisponibilidad()
    {
        return Estado && Existencias > 0;
    }

    public void ActualizarDetalles(string nombre, string descripcion, decimal precio, int idCategoria)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(nombre));

        if (precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.", nameof(precio));

        if (idCategoria <= 0)
            throw new ArgumentException("Debe especificarse una categoría válida.", nameof(idCategoria));

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
        Precio = precio;
        IdCategoria = idCategoria;
    }

    public void CambiarEstado(bool nuevoEstado)
    {
        Estado = nuevoEstado;
    }
}
