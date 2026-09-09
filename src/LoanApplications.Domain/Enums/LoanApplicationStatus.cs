namespace LoanApplications.Domain.Enums;

public enum LoanApplicationStatus
{
    PendingEvaluation = 1,
    PreApproved = 2,
    ManualReview = 3,
    Rejected = 4,
    Failed = 5,
}