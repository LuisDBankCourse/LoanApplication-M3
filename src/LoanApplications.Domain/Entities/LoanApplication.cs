using LoanApplications.Domain.Enums;
using LoanApplications.Domain.Exceptions;

namespace LoanApplications.Domain.Entities;

public class LoanApplication
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CreditProductId { get; set; }
    
    public decimal RequestedAmount { get; set; }
    public int TermMonths { get; set; }
    public string Purpose { get; set; }
    public LoanApplicationStatus Status { get; set; }
    public string IdempotencyKey { get; set; }
    public string CorrelationId { get; set; }
    
    //Fields populated after the evaluation
    public int? Score { get; set; }
    public string? RiskLevel { get; set; }
    public string? DecisionReason { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? EvaluatedAtUtc { get; set; }
    
    public static LoanApplication Create(
        Guid customerId, 
        Guid creditProductId, 
        decimal requestedAmount, 
        int termMonths, 
        string purpose,
        string idempotencyKey,
        string correlationId)
    {

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer Id is required", nameof(customerId));
        }
        if (creditProductId == Guid.Empty)
        {
            throw new ArgumentException("Credit product Id is required", nameof(creditProductId));
        }
        
        if (requestedAmount <= 0)
            throw new ArgumentOutOfRangeException((nameof(requestedAmount)), "requested amount must be greater than 0");
        
        if (termMonths <= 0)
            throw new ArgumentOutOfRangeException((nameof(termMonths)), "termMonths must be greater than 0");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(purpose);
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        return new LoanApplication
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CreditProductId = creditProductId,
            RequestedAmount = requestedAmount,
            TermMonths = termMonths,
            Purpose = purpose.Trim(),
            Status = LoanApplicationStatus.PendingEvaluation,
            IdempotencyKey = idempotencyKey.Trim(),
            CorrelationId = correlationId.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void ApplyDecision(int score, string riskLevel, string decision, string reason, string correlationId)
    {
        if (Status != LoanApplicationStatus.PendingEvaluation)
            throw new InvalidLoanApplicationStateException(Id, Status, "ApplyDecision");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(riskLevel);
        ArgumentException.ThrowIfNullOrWhiteSpace(decision);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        
        if (!Enum.TryParse<LoanApplicationStatus>(decision, true, out var newStatus))
            throw new ArgumentException($"Decision {decision} is not valid for LoanApplicationStatus", nameof(decision));

        if (newStatus is not (LoanApplicationStatus.PreApproved
            or LoanApplicationStatus.ManualReview
            or LoanApplicationStatus.Rejected
            or LoanApplicationStatus.Failed))
        {
            throw new ArgumentException($"Status {newStatus} is not an allowed decision status", nameof(decision));
        }
        
        Score = score;
        RiskLevel = riskLevel;
        DecisionReason = reason;
        Status = newStatus;
        EvaluatedAtUtc = DateTime.UtcNow;
    }
}