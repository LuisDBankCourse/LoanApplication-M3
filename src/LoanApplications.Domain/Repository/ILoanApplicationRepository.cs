namespace LoanApplications.Domain.Repository;

public interface ILoanApplicationRepository
{
    Task<Entities.LoanApplication?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Entities.LoanApplication?> GetByIdempotencyKeyAsync(string key, CancellationToken ct = default);
    Task AddAsync(Entities.LoanApplication loanApplication, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}