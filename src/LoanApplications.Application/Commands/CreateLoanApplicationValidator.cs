using FluentValidation;

namespace LoanApplications.Application.Commands;

public class CreateLoanApplicationValidator : AbstractValidator<CreateLoanApplicationCommand>
{
    public CreateLoanApplicationValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required");
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.RequestedAmount)
            .GreaterThan(0).WithMessage("RequestedAmount must be greater than 0")
            .LessThanOrEqualTo(10_000_000)
            .WithMessage("The requested amount cannot exceed 10,000,000");
        RuleFor(x => x.TermMonths)
            .GreaterThan(0).WithMessage("The requested term must be greater than 0")
            .LessThanOrEqualTo(360).WithMessage("The requested term cannot exceed 360 months");
        RuleFor(x => x.Purpose)
            .NotEmpty().WithMessage("Purpose is required");
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("IdempotencyKey is required");
        RuleFor(x => x.CorrelationId)
            .NotEmpty().WithMessage("CorrelationId is required");
    }
}