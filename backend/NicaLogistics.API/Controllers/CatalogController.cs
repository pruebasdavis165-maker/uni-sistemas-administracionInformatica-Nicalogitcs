using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicaLogistics.Application.UseCases.Catalog;

namespace NicaLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalog(CancellationToken cancellationToken)
    {
        var productos = await _catalogService.GetCatalogAsync(cancellationToken);
        return Ok(productos);
    }

    [HttpPut("{id:int}/inventario")]
    [Authorize]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateInventario(
        [FromRoute] int id,
        [FromBody] UpdateInventarioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var productoActualizado = await _catalogService.UpdateInventarioAsync(id, request, cancellationToken);
            return Ok(productoActualizado);
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
}
