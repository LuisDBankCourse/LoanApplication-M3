using LoanApplications.Domain.Entities;
using LoanApplications.Domain.Repository;
using LoanApplications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LoanApplications.Infrastructure.Repository;

public class OutBoxRepository : IOutBoxRepository
{
    private  readonly LoanApplicationDbContext _dbContext;
    
    public OutBoxRepository(LoanApplicationDbContext dbContext)
    {
      _dbContext = dbContext;  
    }
    public async Task AddAsync(OutBoxMessage outBoxMessage, CancellationToken ct = default)
    => await _dbContext.OutBoxMessages.AddAsync(outBoxMessage, ct);

    public async Task<IReadOnlyList<OutBoxMessage>> GetPendingAsync(int batchSize, CancellationToken ct = default)
        => await _dbContext.OutBoxMessages
            .Where(m => m.ProcessedAtUtc == null && m.RetryCount < 5)
            .OrderByDescending(m => m.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
}