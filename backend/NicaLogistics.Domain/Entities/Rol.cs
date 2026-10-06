namespace NicaLogistics.Domain.Entities;

public class Rol
{
    private readonly List<Usuario> _usuarios = new();

    public int IdRol { get; private set; }
    public string NombreRol { get; private set; } = string.Empty;
    public IReadOnlyCollection<Usuario> Usuarios => _usuarios.AsReadOnly();

    // Constructor protegido para EF Core
    protected Rol() { }

    public Rol(string nombreRol)
    {
        if (string.IsNullOrWhiteSpace(nombreRol))
            throw new ArgumentException("El nombre del rol no puede estar vacío.", nameof(nombreRol));

        NombreRol = nombreRol.Trim();
    }

    public Rol(int idRol, string nombreRol) : this(nombreRol)
    {
        IdRol = idRol;
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nombre del rol no puede estar vacío.", nameof(nuevoNombre));

        NombreRol = nuevoNombre.Trim();
    }
}
