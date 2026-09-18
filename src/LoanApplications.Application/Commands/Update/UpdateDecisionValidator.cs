using FluentValidation;

namespace LoanApplications.Application.Commands.Update
{
    public class UpdateDecisionValidator : AbstractValidator<UpdateDecisionCommand>
    {
        private static readonly string[] ValidDecisions = { "PreApproved", "ManualReview", "Rejected", "Failed" };

        public UpdateDecisionValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithMessage("ApplicationId is required");

            RuleFor(x => x.Score)
                .InclusiveBetween(0, 120)
                .WithMessage("Score must be between 0 and 120");

            RuleFor(x => x.RiskLevel)
                .NotEmpty().WithMessage("RiskLevel is required")
                .Must(r => new[] { "Low", "Medium", "High" }.Contains(r))
                .WithMessage("RiskLevel must be Low, Medium, or High");

            RuleFor(x => x.Decision)
                .NotEmpty().WithMessage("Decision is required")
                .Must(d => ValidDecisions.Contains(d))
                .WithMessage($"Decision must be one of the following: {string.Join(", ", ValidDecisions)}");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required");

            RuleFor(x => x.CorrelationId)
                .NotEmpty().WithMessage("CorrelationId is required");
        }
    }
}