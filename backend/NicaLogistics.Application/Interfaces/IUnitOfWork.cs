namespace NicaLogistics.Application.Interfaces;

/// <summary>
/// Permite ejecutar varias operaciones de repositorio como una única transacción atómica.
/// </summary>
public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
