using LoanApplications.Application.Common;
using MediatR;

namespace LoanApplications.Application.Commands.Update;

public record UpdateDecisionCommand
(
    Guid ApplicationId,
    int Score,
    string RiskLevel,
    string Decision,
    string Reason,
    string CorrelationId

):IRequest<Result<UpdateDecisionResponse>>;


public record UpdateDecisionResponse
(
    Guid ApplicationId,
    string Status,
    string Message

);