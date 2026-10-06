using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicaLogistics.Application.UseCases.Orders;

namespace NicaLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CrearPedido([FromBody] CrearPedidoRequest request, CancellationToken cancellationToken)
    {
        var idCliente = ObtenerUsuarioId();
        if (idCliente <= 0)
            return Unauthorized(new { message = "Usuario no autenticado o token inválido." });

        try
        {
            var pedido = await _pedidoService.CrearPedidoAsync(idCliente, request, cancellationToken);
            return CreatedAtAction(nameof(GetPedidoPorId), new { id = pedido.IdPedido }, pedido);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("mis-pedidos")]
    [ProducesResponseType(typeof(IReadOnlyList<PedidoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMisPedidos(CancellationToken cancellationToken)
    {
        var idCliente = ObtenerUsuarioId();
        if (idCliente <= 0)
            return Unauthorized(new { message = "Usuario no autenticado o token inválido." });

        var pedidos = await _pedidoService.GetMisPedidosAsync(idCliente, cancellationToken);
        return Ok(pedidos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPedidoPorId([FromRoute] int id, CancellationToken cancellationToken)
    {
        var idUsuario = ObtenerUsuarioId();
        var esAdmin = User.IsInRole("Administrador");

        try
        {
            var pedido = await _pedidoService.GetByIdAsync(id, cancellationToken);

            // Solo el cliente dueño del pedido o un administrador pueden ver el detalle
            if (!esAdmin && pedido.IdCliente != idUsuario)
            {
                return Forbid();
            }

            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/estado-envio")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarEstadoEnvio(
        [FromRoute] int id,
        [FromBody] ActualizarEstadoEnvioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var pedidoActualizado = await _pedidoService.ActualizarEstadoEnvioAsync(id, request, cancellationToken);
            return Ok(pedidoActualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) 
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                    ?? User.FindFirst("id");

        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }
}
