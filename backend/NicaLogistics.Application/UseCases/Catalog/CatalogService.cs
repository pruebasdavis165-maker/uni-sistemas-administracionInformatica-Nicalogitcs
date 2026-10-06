using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.UseCases.Catalog;

public class CatalogService : ICatalogService
{
    private readonly IProductoRepository _productoRepository;

    public CatalogService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
    }

    public async Task<IReadOnlyList<ProductoDto>> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        var productos = await _productoRepository.GetActivosAsync(cancellationToken);

        return productos.Select(p => MapToDto(p)).ToList();
    }

    public async Task<ProductoDto> UpdateInventarioAsync(
        int idProducto,
        UpdateInventarioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (idProducto <= 0)
            throw new ArgumentException("El ID del producto debe ser un entero positivo.", nameof(idProducto));

        var producto = await _productoRepository.GetByIdAsync(idProducto, cancellationToken);
        if (producto is null)
            throw new KeyNotFoundException($"El producto con ID {idProducto} no fue encontrado.");

        producto.ActualizarExistencias(request.Cantidad);

        await _productoRepository.UpdateAsync(producto, cancellationToken);

        return MapToDto(producto);
    }

    private static ProductoDto MapToDto(Producto producto)
    {
        return new ProductoDto(
            IdProducto: producto.IdProducto,
            Nombre: producto.Nombre,
            Descripcion: producto.Descripcion,
            Precio: producto.Precio,
            Existencias: producto.Existencias,
            Estado: producto.Estado,
            IdCategoria: producto.IdCategoria,
            NombreCategoria: producto.Categoria?.NombreCategoria ?? string.Empty
        );
    }
}
