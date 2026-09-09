using System.Text.Json;
using LoanApplications.Application.Common;
using LoanApplications.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LoanApplications.Application.Commands;

public class CreateLoanApplicationHandler : IRequestHandler<CreateLoanApplicationCommand, Result<CreateLoanApplicationResponse>>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly IOutBoxRepository _outboxRepository;
    private readonly ILogger _logger;
    
    public CreateLoanApplicationHandler(
        ILoanApplicationRepository loanApplicationRepository,
        IOutBoxRepository outboxRepository,
        ILogger<CreateLoanApplicationHandler> logger)
    {
        _loanApplicationRepository = loanApplicationRepository;
        _outboxRepository = outboxRepository;
        _logger = logger;
    }

    public async Task<Result<CreateLoanApplicationResponse>> Handle(CreateLoanApplicationCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Starting loan application creation CustomerId={CustomerId}, ProductId={ProductId}, RequestedAmount={RequestedAmount}",
            request.CustomerId, request.ProductId, request.RequestedAmount);
        var exists = await _loanApplicationRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey);
        if (exists is not null)
        {
            _logger.LogWarning("Duplicate IdempotencyKey LoanApplicationId={Id}, IdempotencyKey={IdempotencyKey}",
                exists.Id, request.IdempotencyKey);
            return Result<CreateLoanApplicationResponse>.Failure($"Loan application with IdempotencyKey {request.IdempotencyKey} already exists");
        }
        var application = Domain.Entities.LoanApplication.Create(
            request.CustomerId,
            request.ProductId,
            request.RequestedAmount,
            request.TermMonths,
            request.Purpose,
            request.IdempotencyKey,
            request.CorrelationId
        );
        
        var @event = new LoanApplicationCreatedEvent(
            ApplicationId: application.Id,
            CustomerId: application.CustomerId,
            ProductId: application.CreditProductId,
            RequestedAmount: application.RequestedAmount,
            TermMonths: application.TermMonths,
            Purpose: application.Purpose,
            OccurredAtUtc: application.CreatedAtUtc,
            CorrelationId: application.CorrelationId);
        var payload = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var outboxMessage = Domain.Entities.OutBoxMessage.Create(
            eventType: nameof(LoanApplicationCreatedEvent),
            payload: payload,
            correlationId: application.CorrelationId
        );
        await _loanApplicationRepository.AddAsync(application, ct);
        await _outboxRepository.AddAsync(outboxMessage, ct);
        await _loanApplicationRepository.SaveChangesAsync(ct);
        
        _logger.LogInformation("Application created and enqueued. ApplicationId={application}, outboxMessageId={outboxMessageId}", application.Id, outboxMessage.Id);
        return Result<CreateLoanApplicationResponse>.Success(new CreateLoanApplicationResponse(
            application.Id,
            application.RequestedAmount,
            application.Status.ToString(),
            "Application registered successfully",
            request.CorrelationId));
    }
    
    public record LoanApplicationCreatedEvent
    (
        Guid ApplicationId,
        Guid CustomerId,
        Guid ProductId,
        decimal RequestedAmount,
        int TermMonths,
        string Purpose,
        DateTime OccurredAtUtc,
        string CorrelationId
    );
}