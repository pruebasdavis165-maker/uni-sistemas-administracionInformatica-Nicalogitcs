using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicaLogistics.Application.UseCases.Payments;

namespace NicaLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly IPagoService _pagoService;

    public PagosController(IPagoService pagoService)
    {
        _pagoService = pagoService ?? throw new ArgumentNullException(nameof(pagoService));
    }

    /// <summary>Registra y confirma el pago de un pedido; genera automáticamente la encomienda.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RegistrarPagoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoRequest request, CancellationToken ct) =>
        Ejecutar(async () =>
        {
            var response = await _pagoService.RegistrarPagoAsync(ObtenerUsuarioId(), EsAdmin(), request, ct);
            return CreatedAtAction(nameof(GetEstado), new { id = response.Pago.IdPago }, response);
        });

    [HttpGet("{id:int}/estado")]
    [ProducesResponseType(typeof(PagoDto), StatusCodes.Status200OK)]
    public Task<IActionResult> GetEstado([FromRoute] int id, CancellationToken ct) =>
        Ejecutar(async () => Ok(await _pagoService.GetEstadoAsync(id, ObtenerUsuarioId(), EsAdmin(), ct)));

    [HttpGet("mis-pagos")]
    [ProducesResponseType(typeof(IReadOnlyList<PagoDto>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetMisPagos(CancellationToken ct) =>
        Ejecutar(async () => Ok(await _pagoService.GetMisPagosAsync(ObtenerUsuarioId(), ct)));

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(IReadOnlyList<PagoDto>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetTodos(CancellationToken ct) =>
        Ejecutar(async () => Ok(await _pagoService.GetTodosAsync(ct)));

    [HttpPost("{id:int}/cancelar")]
    [ProducesResponseType(typeof(PagoDto), StatusCodes.Status200OK)]
    public Task<IActionResult> Cancelar([FromRoute] int id, CancellationToken ct) =>
        Ejecutar(async () => Ok(await _pagoService.CancelarPagoAsync(id, ObtenerUsuarioId(), EsAdmin(), ct)));

    [HttpPost("{id:int}/reembolso")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(PagoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Reembolsar([FromRoute] int id, CancellationToken ct) =>
        Ejecutar(async () => Ok(await _pagoService.ProcesarReembolsoAsync(id, ct)));

    private async Task<IActionResult> Ejecutar(Func<Task<IActionResult>> accion)
    {
        try
        {
            return await accion();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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

    private bool EsAdmin() => User.IsInRole("Administrador");

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }
}
