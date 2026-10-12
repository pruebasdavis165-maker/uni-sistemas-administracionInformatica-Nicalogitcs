using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicaLogistics.Application.UseCases.Encomiendas;

namespace NicaLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EncomiendasController : ControllerBase
{
    private readonly IEncomiendaService _encomiendaService;

    public EncomiendasController(IEncomiendaService encomiendaService)
    {
        _encomiendaService = encomiendaService ?? throw new ArgumentNullException(nameof(encomiendaService));
    }

    /// <summary>Encomiendas de los pedidos del usuario autenticado.</summary>
    [HttpGet("propia")]
    [ProducesResponseType(typeof(IReadOnlyList<EncomiendaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPropias(CancellationToken ct)
    {
        var idUsuario = ObtenerUsuarioId();
        if (idUsuario <= 0)
            return Unauthorized(new { message = "Usuario no autenticado o token inválido." });

        return Ok(await _encomiendaService.GetPropiasAsync(idUsuario, ct));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(IReadOnlyList<EncomiendaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodas(CancellationToken ct) =>
        Ok(await _encomiendaService.GetTodasAsync(ct));

    [HttpPut("{id:int}/estado")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(EncomiendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarEstado(
        [FromRoute] int id,
        [FromBody] ActualizarEstadoEncomiendaRequest request,
        CancellationToken ct)
    {
        try
        {
            return Ok(await _encomiendaService.ActualizarEstadoAsync(id, request, ct));
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
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }
}
