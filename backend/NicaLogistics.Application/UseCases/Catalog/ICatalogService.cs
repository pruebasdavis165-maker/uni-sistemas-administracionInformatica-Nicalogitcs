namespace NicaLogistics.Application.UseCases.Catalog;

public interface ICatalogService
{
    Task<IReadOnlyList<ProductoDto>> GetCatalogAsync(CancellationToken cancellationToken = default);
    Task<ProductoDto> UpdateInventarioAsync(int idProducto, UpdateInventarioRequest request, CancellationToken cancellationToken = default);
}
