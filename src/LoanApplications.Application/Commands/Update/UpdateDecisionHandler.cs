using LoanApplications.Application.Common;
using LoanApplications.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LoanApplications.Application.Commands.Update;

public class UpdateDecisionHandler : IRequestHandler<UpdateDecisionCommand, Result<UpdateDecisionResponse>>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ILogger<UpdateDecisionHandler> _logger;

        public UpdateDecisionHandler(ILoanApplicationRepository loanApplicationRepository,
            ILogger<UpdateDecisionHandler> logger)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _logger = logger;
        }


        public async Task<Result<UpdateDecisionResponse>> Handle(UpdateDecisionCommand request, CancellationToken ct)
        {
            _logger.LogInformation(
                "Starting application update. ApplicationId={ApplicationId}, Score={Score}, Decision={Decision}",
                request.ApplicationId, request.Score, request.Decision);
            var application = await _loanApplicationRepository.GetByIdAsync(request.ApplicationId);
            if (application is null)
            {
                _logger.LogError(
                    "Application does not exist. ApplicationId={ApplicationId}",
                    request.ApplicationId);
                return Result<UpdateDecisionResponse>.Failure(
                    $"Application with ID '{request.ApplicationId}' does not exist");
            }
            application.ApplyDecision(
                request.Score,
                request.RiskLevel,
                request.Decision,
                request.Reason,
                request.CorrelationId
            );
            await _loanApplicationRepository.SaveChangesAsync(ct);
            _logger.LogInformation(
                "Application updated successfully. ApplicationId={ApplicationId}, Score={Score}, Status={Status}",
                application.Id, application.Score, application.Status);
            return Result<UpdateDecisionResponse>.Success(new UpdateDecisionResponse(
                application.Id,
                application.Status.ToString(),
                "Application updated successfully"));
        }
    }