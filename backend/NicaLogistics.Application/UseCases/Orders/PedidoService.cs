using NicaLogistics.Application.Interfaces;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.UseCases.Orders;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProductoRepository _productoRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProductoRepository productoRepository)
    {
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
    }

    public async Task<PedidoDto> CrearPedidoAsync(
        int idCliente,
        CrearPedidoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (idCliente <= 0)
            throw new ArgumentException("El ID del cliente debe ser válido.", nameof(idCliente));

        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException("El pedido debe contener al menos un producto.", nameof(request.Items));

        if (string.IsNullOrWhiteSpace(request.DireccionDestino))
            throw new ArgumentException("La dirección de destino es obligatoria.", nameof(request.DireccionDestino));

        var nuevoPedido = new Pedido(idCliente);

        // 1. Validar existencias y preparar productos
        var productosParaActualizar = new List<(Producto Producto, int Cantidad)>();

        foreach (var item in request.Items)
        {
            if (item.Cantidad <= 0)
                throw new ArgumentException($"La cantidad para el producto ID {item.IdProducto} debe ser mayor que cero.");

            var producto = await _productoRepository.GetByIdAsync(item.IdProducto, cancellationToken);
            if (producto is null)
                throw new KeyNotFoundException($"El producto con ID {item.IdProducto} no existe.");

            if (!producto.Estado)
                throw new InvalidOperationException($"El producto '{producto.Nombre}' no está disponible actualmente.");

            if (producto.Existencias < item.Cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente para el producto '{producto.Nombre}'. Disponibles: {producto.Existencias}, solicitados: {item.Cantidad}.");

            productosParaActualizar.Add((producto, item.Cantidad));
            nuevoPedido.AgregarDetalle(producto.IdProducto, item.Cantidad, producto.Precio);
        }

        // 2. Descontar inventario automáticamente
        foreach (var (producto, cantidad) in productosParaActualizar)
        {
            producto.DisminuirExistencias(cantidad);
            await _productoRepository.UpdateAsync(producto, cancellationToken);
        }

        // 3. Generar Envio asociado con guía de rastreo única
        var guiaRastreo = $"NL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var fechaEstimada = DateTime.UtcNow.AddDays(3); // Estimado estándar de entrega

        var envio = new Envio(
            direccionDestino: request.DireccionDestino,
            guiaRastreo: guiaRastreo,
            fechaEstimada: fechaEstimada,
            estadoEnvio: "EnPreparacion"
        );

        nuevoPedido.AsignarEnvio(envio);

        // 4. Guardar pedido en la base de datos
        await _pedidoRepository.AddAsync(nuevoPedido, cancellationToken);

        // Recargar con navegación completa para el DTO
        var pedidoCreado = await _pedidoRepository.GetByIdAsync(nuevoPedido.IdPedido, cancellationToken);
        return MapToDto(pedidoCreado ?? nuevoPedido);
    }

    public async Task<PedidoDto> GetByIdAsync(int idPedido, CancellationToken cancellationToken = default)
    {
        if (idPedido <= 0)
            throw new ArgumentException("El ID del pedido debe ser válido.", nameof(idPedido));

        var pedido = await _pedidoRepository.GetByIdAsync(idPedido, cancellationToken);
        if (pedido is null)
            throw new KeyNotFoundException($"El pedido con ID {idPedido} no fue encontrado.");

        return MapToDto(pedido);
    }

    public async Task<IReadOnlyList<PedidoDto>> GetMisPedidosAsync(int idCliente, CancellationToken cancellationToken = default)
    {
        if (idCliente <= 0)
            throw new ArgumentException("El ID del cliente debe ser válido.", nameof(idCliente));

        var pedidos = await _pedidoRepository.GetByClienteIdAsync(idCliente, cancellationToken);
        return pedidos.Select(p => MapToDto(p)).ToList();
    }

    public async Task<PedidoDto> ActualizarEstadoEnvioAsync(
        int idPedido,
        ActualizarEstadoEnvioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (idPedido <= 0)
            throw new ArgumentException("El ID del pedido debe ser válido.", nameof(idPedido));

        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.NuevoEstado))
            throw new ArgumentException("El nuevo estado no puede estar vacío.", nameof(request.NuevoEstado));

        var pedido = await _pedidoRepository.GetByIdAsync(idPedido, cancellationToken);
        if (pedido is null)
            throw new KeyNotFoundException($"El pedido con ID {idPedido} no fue encontrado.");

        if (pedido.Envio is null)
            throw new InvalidOperationException($"El pedido con ID {idPedido} no cuenta con un registro de envío asociado.");

        pedido.Envio.ActualizarEstado(request.NuevoEstado);

        if (request.FechaEstimada.HasValue)
        {
            pedido.Envio.ActualizarFechaEstimada(request.FechaEstimada.Value);
        }

        // Sincronizar estado del pedido con el estado del envío si corresponde
        if (request.NuevoEstado.Equals("Entregado", StringComparison.OrdinalIgnoreCase))
        {
            pedido.ActualizarEstado("Entregado");
        }
        else if (request.NuevoEstado.Equals("EnTransito", StringComparison.OrdinalIgnoreCase))
        {
            pedido.ActualizarEstado("Enviado");
        }

        await _pedidoRepository.UpdateAsync(pedido, cancellationToken);

        return MapToDto(pedido);
    }

    private static PedidoDto MapToDto(Pedido pedido)
    {
        var detallesDto = pedido.Detalles.Select(d => new DetallePedidoDto(
            IdDetalle: d.IdDetalle,
            IdProducto: d.IdProducto,
            NombreProducto: d.Producto?.Nombre ?? $"Producto #{d.IdProducto}",
            Cantidad: d.Cantidad,
            PrecioUnitario: d.PrecioUnitario,
            Subtotal: d.Subtotal
        )).ToList();

        EnvioDto? envioDto = null;
        if (pedido.Envio != null)
        {
            envioDto = new EnvioDto(
                IdEnvio: pedido.Envio.IdEnvio,
                IdPedido: pedido.Envio.IdPedido,
                DireccionDestino: pedido.Envio.DireccionDestino,
                EstadoEnvio: pedido.Envio.EstadoEnvio,
                FechaEstimada: pedido.Envio.FechaEstimada,
                GuiaRastreo: pedido.Envio.GuiaRastreo
            );
        }

        return new PedidoDto(
            IdPedido: pedido.IdPedido,
            FechaPedido: pedido.FechaPedido,
            Estado: pedido.Estado,
            Total: pedido.Total,
            IdCliente: pedido.IdCliente,
            Detalles: detallesDto,
            Envio: envioDto
        );
    }
}
