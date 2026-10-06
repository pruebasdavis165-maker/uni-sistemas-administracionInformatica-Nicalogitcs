namespace NicaLogistics.Domain.Entities;

public class Categoria
{
    private readonly List<Producto> _productos = new();

    public int IdCategoria { get; private set; }
    public string NombreCategoria { get; private set; } = string.Empty;
    public IReadOnlyCollection<Producto> Productos => _productos.AsReadOnly();

    // Constructor protegido para EF Core
    protected Categoria() { }

    public Categoria(string nombreCategoria)
    {
        if (string.IsNullOrWhiteSpace(nombreCategoria))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nombreCategoria));

        NombreCategoria = nombreCategoria.Trim();
    }

    public Categoria(int idCategoria, string nombreCategoria) : this(nombreCategoria)
    {
        IdCategoria = idCategoria;
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nuevoNombre));

        NombreCategoria = nuevoNombre.Trim();
    }
}
