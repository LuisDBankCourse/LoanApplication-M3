using LoanApplications.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanApplications.Infrastructure.Persistence;

public class LoanApplicationDbContext : DbContext
{
    public LoanApplicationDbContext(DbContextOptions<LoanApplicationDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<LoanApplication> LoanApplications { get; set; }
    public DbSet<OutBoxMessage>  OutBoxMessages { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LoanApplication>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.RequestedAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            e.Property(x => x.Purpose)
                .HasMaxLength(500)
                .IsRequired();
            
            e.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            
            e.Property(x => x.IdempotencyKey)
                .HasMaxLength(100)
                .IsRequired();
            
            e.HasIndex(x => x.IdempotencyKey)
                .IsUnique()
                .HasDatabaseName("UQ_Loan_IdempotencyKey");
            
            e.Property(x => x.CorrelationId)
                .HasMaxLength(100)
                .IsRequired();
            
            e.HasIndex(x => x.CorrelationId)
                .IsUnique()
                .HasDatabaseName("UQ_Loan_CorrelationId");

            e.Property(x => x.RiskLevel)
                .HasMaxLength(20);
            
            e.Property(x => x.DecisionReason)
                .HasMaxLength(500);
            
        });

        modelBuilder.Entity<OutBoxMessage>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.EventType)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.Payload)
                .IsRequired();

            e.Property(e => e.CorrelationId)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(e => e.Error)
                .HasMaxLength(7000);

            e.HasIndex(x => x.ProcessedAtUtc)
                .HasDatabaseName("IX_OutBox_ProcessedAtUtc");
        });
    }
}