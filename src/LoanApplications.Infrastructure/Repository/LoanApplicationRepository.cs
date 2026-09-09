using LoanApplications.Domain.Entities;
using LoanApplications.Domain.Repository;
using LoanApplications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LoanApplications.Infrastructure.Repository;

public class LoanApplicationRepository : ILoanApplicationRepository
{
    private readonly LoanApplicationDbContext _dbContext;
    
    public LoanApplicationRepository(LoanApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoanApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
      =>  await _dbContext.LoanApplications.FirstOrDefaultAsync(x => x.Id == id, ct);
    

    public async Task<LoanApplication?> GetByIdempotencyKeyAsync(string key, CancellationToken ct = default)
        => await _dbContext.LoanApplications.FirstOrDefaultAsync(x => x.IdempotencyKey == key, ct);

    public async Task AddAsync(LoanApplication loanApplication, CancellationToken ct = default)
        => await _dbContext.LoanApplications.AddAsync(loanApplication, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
    => _dbContext.SaveChangesAsync(ct);
}