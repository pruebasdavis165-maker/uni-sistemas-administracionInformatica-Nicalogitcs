namespace NicaLogistics.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; protected set; }
    public string Nombre { get; protected set; } = string.Empty;
    public string Correo { get; protected set; } = string.Empty;
    public string Contrasena { get; protected set; } = string.Empty;
    public bool Estado { get; protected set; }
    public int IdRol { get; protected set; }
    public Rol? Rol { get; protected set; }

    // Constructor protegido para EF Core y clases derivadas
    protected Usuario() { }

    public Usuario(string nombre, string correo, string contrasena, int idRol, bool estado = true)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(correo))
            throw new ArgumentException("El correo no puede estar vacío.", nameof(correo));

        if (string.IsNullOrWhiteSpace(contrasena))
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contrasena));

        if (idRol <= 0)
            throw new ArgumentException("Debe especificarse un rol válido.", nameof(idRol));

        Nombre = nombre.Trim();
        Correo = correo.Trim().ToLowerInvariant();
        Contrasena = contrasena;
        IdRol = idRol;
        Estado = estado;
    }

    public Usuario(int idUsuario, string nombre, string correo, string contrasena, int idRol, bool estado = true)
        : this(nombre, correo, contrasena, idRol, estado)
    {
        IdUsuario = idUsuario;
    }

    public void ActualizarPerfil(string nombre, string correo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(correo))
            throw new ArgumentException("El correo no puede estar vacío.", nameof(correo));

        Nombre = nombre.Trim();
        Correo = correo.Trim().ToLowerInvariant();
    }

    public void ActualizarContrasena(string nuevaContrasena)
    {
        if (string.IsNullOrWhiteSpace(nuevaContrasena))
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(nuevaContrasena));

        Contrasena = nuevaContrasena;
    }

    public void AsignarRol(int idRol)
    {
        if (idRol <= 0)
            throw new ArgumentException("El rol debe ser válido.", nameof(idRol));

        IdRol = idRol;
    }

    public void CambiarEstado(bool nuevoEstado)
    {
        Estado = nuevoEstado;
    }
}
