using LoanApplications.Application.Common;
using MediatR;

namespace LoanApplications.Application.Commands;

public record CreateLoanApplicationCommand(
    Guid CustomerId,
    Guid ProductId,
    decimal RequestedAmount,
    int TermMonths,
    string Purpose,
    string IdempotencyKey,
    string CorrelationId
): IRequest<Result<CreateLoanApplicationResponse>>;

public record CreateLoanApplicationResponse(
    Guid ApplicationId,
    decimal RequestedAmount,
    string Status,
    string Message,
    string CorrelationId
);
