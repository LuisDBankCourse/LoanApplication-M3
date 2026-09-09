namespace LoanApplications.Domain.Repository;

public interface IOutBoxRepository
{
    Task AddAsync(Entities.OutBoxMessage outBoxMessage, CancellationToken ct = default);
    Task<IReadOnlyList<Entities.OutBoxMessage>> GetPendingAsync(int batchSize, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}