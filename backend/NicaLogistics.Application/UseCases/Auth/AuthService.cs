using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.UseCases.Auth;

public class AuthService : IAuthService
{
    private const int RolClienteId = 2; // Id correspondiente al rol 'Cliente' definido en el seeder

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasena))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var usuario = await _usuarioRepository.GetByCorreoAsync(request.Correo, cancellationToken);
        if (usuario is null)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (!usuario.Estado)
            throw new UnauthorizedAccessException("La cuenta de usuario se encuentra inactiva.");

        var passwordValida = _passwordHasher.Verify(request.Contrasena, usuario.Contrasena);
        if (!passwordValida)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var token = _jwtProvider.Generate(usuario);
        return new LoginResponse(token);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(request.Nombre));

        if (string.IsNullOrWhiteSpace(request.Correo))
            throw new ArgumentException("El correo no puede estar vacío.", nameof(request.Correo));

        if (string.IsNullOrWhiteSpace(request.Contrasena))
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(request.Contrasena));

        var existeUsuario = await _usuarioRepository.GetByCorreoAsync(request.Correo, cancellationToken);
        if (existeUsuario != null)
            throw new InvalidOperationException("El correo electrónico ya se encuentra registrado.");

        var passwordHash = _passwordHasher.Hash(request.Contrasena);

        // Se crea el usuario asignando por defecto el rol de 'Cliente' (Id 2) y estado activo
        var nuevoUsuario = new Usuario(
            nombre: request.Nombre,
            correo: request.Correo,
            contrasena: passwordHash,
            idRol: RolClienteId,
            estado: true
        );

        await _usuarioRepository.AddAsync(nuevoUsuario, cancellationToken);

        var token = _jwtProvider.Generate(nuevoUsuario);
        return new LoginResponse(token);
    }
}
