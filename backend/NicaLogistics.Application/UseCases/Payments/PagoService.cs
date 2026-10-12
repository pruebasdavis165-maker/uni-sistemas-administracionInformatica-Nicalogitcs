using NicaLogistics.Application.Interfaces;
using NicaLogistics.Application.UseCases.Encomiendas;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Application.UseCases.Payments;

public class PagoService : IPagoService
{
    // Origen por defecto de las encomiendas generadas automáticamente.
    private const string OrigenPorDefecto = "Centro de Distribución NicaLogistics, Managua";

    private readonly IPagoRepository _pagoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IEncomiendaRepository _encomiendaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PagoService(
        IPagoRepository pagoRepository,
        IPedidoRepository pedidoRepository,
        IEncomiendaRepository encomiendaRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _pagoRepository = pagoRepository ?? throw new ArgumentNullException(nameof(pagoRepository));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _encomiendaRepository = encomiendaRepository ?? throw new ArgumentNullException(nameof(encomiendaRepository));
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<RegistrarPagoResponse> RegistrarPagoAsync(
        int idUsuario,
        bool esAdmin,
        RegistrarPagoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (request.IdPedido <= 0)
            throw new ArgumentException("El ID del pedido debe ser válido.", nameof(request.IdPedido));

        var pedido = await _pedidoRepository.GetByIdAsync(request.IdPedido, cancellationToken)
            ?? throw new KeyNotFoundException($"El pedido con ID {request.IdPedido} no fue encontrado.");

        if (!esAdmin && pedido.IdCliente != idUsuario)
            throw new UnauthorizedAccessException("No puede pagar un pedido que no le pertenece.");

        if (pedido.Estado != "Pendiente")
            throw new InvalidOperationException(
                $"Solo se pueden pagar pedidos en estado Pendiente (estado actual: {pedido.Estado}).");

        if (pedido.Envio is null)
            throw new InvalidOperationException("El pedido no tiene un envío asociado; no se puede generar la encomienda.");

        var pagosPrevios = await _pagoRepository.GetByPedidoIdAsync(pedido.IdPedido, cancellationToken);
        if (pagosPrevios.Any(p => p.Estado is Pago.EstadoPendiente or Pago.EstadoCompletado))
            throw new InvalidOperationException("El pedido ya tiene un pago registrado.");

        var pago = Pago.Registrar(pedido.IdPedido, pedido.Total, request.MetodoPago);
        pago.Confirmar();

        Encomienda? encomienda = null;

        // Pago + cambio de estado del pedido + encomienda: todo o nada.
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _pagoRepository.AddAsync(pago, cancellationToken);

            pedido.ActualizarEstado("Procesando");
            await _pedidoRepository.UpdateAsync(pedido, cancellationToken);

            // Automatización: generar la encomienda (idempotente por pedido).
            encomienda = await _encomiendaRepository.GetByPedidoIdAsync(pedido.IdPedido, cancellationToken);
            if (encomienda is null)
            {
                encomienda = new Encomienda(pedido.IdPedido, OrigenPorDefecto, pedido.Envio.DireccionDestino);
                await _encomiendaRepository.AddAsync(encomienda, cancellationToken);
            }
        }, cancellationToken);

        return new RegistrarPagoResponse(MapToDto(pago), EncomiendaMapper.ToDto(encomienda!));
    }

    public async Task<PagoDto> GetEstadoAsync(
        int idPago,
        int idUsuario,
        bool esAdmin,
        CancellationToken cancellationToken = default)
    {
        var pago = await ObtenerPagoAsync(idPago, cancellationToken);
        VerificarAcceso(pago, idUsuario, esAdmin);
        return MapToDto(pago);
    }

    public async Task<IReadOnlyList<PagoDto>> GetMisPagosAsync(int idUsuario, CancellationToken cancellationToken = default)
    {
        var pagos = await _pagoRepository.GetByClienteIdAsync(idUsuario, cancellationToken);
        return pagos.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<PagoDto>> GetTodosAsync(CancellationToken cancellationToken = default)
    {
        var pagos = await _pagoRepository.GetAllAsync(cancellationToken);
        return pagos.Select(MapToDto).ToList();
    }

    public async Task<PagoDto> CancelarPagoAsync(
        int idPago,
        int idUsuario,
        bool esAdmin,
        CancellationToken cancellationToken = default)
    {
        var pago = await ObtenerPagoAsync(idPago, cancellationToken);
        VerificarAcceso(pago, idUsuario, esAdmin);

        pago.Cancelar();
        await _pagoRepository.UpdateAsync(pago, cancellationToken);

        return MapToDto(pago);
    }

    public async Task<PagoDto> ProcesarReembolsoAsync(int idPago, CancellationToken cancellationToken = default)
    {
        var pago = await ObtenerPagoAsync(idPago, cancellationToken);

        var pedido = await _pedidoRepository.GetByIdAsync(pago.IdPedido, cancellationToken)
            ?? throw new KeyNotFoundException($"El pedido con ID {pago.IdPedido} no fue encontrado.");

        // Las validaciones de dominio se ejecutan antes de persistir cualquier cambio.
        pago.ProcesarReembolso();   // solo pagos Completados
        pedido.Cancelar();          // no permite cancelar pedidos Enviados/Entregados

        var encomienda = await _encomiendaRepository.GetByPedidoIdAsync(pedido.IdPedido, cancellationToken);
        if (encomienda is not null && encomienda.Estado != Encomienda.EstadoCancelada)
            encomienda.ActualizarEstado(Encomienda.EstadoCancelada);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _pagoRepository.UpdateAsync(pago, cancellationToken);
            await _pedidoRepository.UpdateAsync(pedido, cancellationToken);

            if (encomienda is not null)
                await _encomiendaRepository.UpdateAsync(encomienda, cancellationToken);

            // Se devuelve el inventario descontado al crear el pedido.
            foreach (var detalle in pedido.Detalles)
            {
                if (detalle.Producto is null) continue;
                detalle.Producto.IncrementarExistencias(detalle.Cantidad);
                await _productoRepository.UpdateAsync(detalle.Producto, cancellationToken);
            }
        }, cancellationToken);

        return MapToDto(pago);
    }

    private async Task<Pago> ObtenerPagoAsync(int idPago, CancellationToken cancellationToken)
    {
        if (idPago <= 0)
            throw new ArgumentException("El ID del pago debe ser válido.", nameof(idPago));

        return await _pagoRepository.GetByIdAsync(idPago, cancellationToken)
            ?? throw new KeyNotFoundException($"El pago con ID {idPago} no fue encontrado.");
    }

    private static void VerificarAcceso(Pago pago, int idUsuario, bool esAdmin)
    {
        if (!esAdmin && pago.Pedido?.IdCliente != idUsuario)
            throw new UnauthorizedAccessException("No tiene acceso a este pago.");
    }

    private static PagoDto MapToDto(Pago p) => new(
        IdPago: p.IdPago,
        IdPedido: p.IdPedido,
        FechaPago: p.FechaPago,
        Monto: p.Monto,
        Estado: p.Estado,
        MetodoPago: p.MetodoPago,
        FechaReembolso: p.FechaReembolso
    );
}
