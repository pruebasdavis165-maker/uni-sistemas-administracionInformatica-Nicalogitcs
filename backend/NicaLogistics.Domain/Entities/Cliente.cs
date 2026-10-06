namespace NicaLogistics.Domain.Entities;

public class Cliente : Usuario
{
    public string Direccion { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;

    // Constructor protegido para EF Core
    protected Cliente() : base() { }

    public Cliente(
        string nombre,
        string correo,
        string contrasena,
        int idRol,
        string direccion,
        string telefono,
        bool estado = true)
        : base(nombre, correo, contrasena, idRol, estado)
    {
        Direccion = direccion?.Trim() ?? string.Empty;
        Telefono = telefono?.Trim() ?? string.Empty;
    }

    public Cliente(
        int idUsuario,
        string nombre,
        string correo,
        string contrasena,
        int idRol,
        string direccion,
        string telefono,
        bool estado = true)
        : base(idUsuario, nombre, correo, contrasena, idRol, estado)
    {
        Direccion = direccion?.Trim() ?? string.Empty;
        Telefono = telefono?.Trim() ?? string.Empty;
    }

    public void ActualizarDatosContacto(string direccion, string telefono)
    {
        Direccion = direccion?.Trim() ?? string.Empty;
        Telefono = telefono?.Trim() ?? string.Empty;
    }
}
