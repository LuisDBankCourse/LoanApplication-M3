using FluentValidation;
using LoanApplications.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LoanApplications.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class LoanApplicationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateLoanApplicationCommand> _validator;
    private readonly ILogger<LoanApplicationController> _logger;

    public LoanApplicationController(
        IMediator mediator,
        IValidator<CreateLoanApplicationCommand> validator,
        ILogger<LoanApplicationController> logger)
    {
        _mediator = mediator;
        _validator = validator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateLoanApplicationResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateLoanApplication([FromBody] CreateLoanApplicationRequest request,
        CancellationToken ct)
    {
        var correlationId = HttpContext.Request.Headers["X-Correlation-Id"]
            .FirstOrDefault() ?? Guid.NewGuid().ToString();
        
        var idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"]
            .FirstOrDefault() ?? Guid.NewGuid().ToString();
        
        var command = new CreateLoanApplicationCommand(
            request.CustomerId,
            request.ProductId,
            request.RequestedAmount,
            request.TermMonths,
            request.Purpose,
            idempotencyKey,
            correlationId
            );
        
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g
                    .Select(x => x.ErrorMessage)
                    .ToArray());
            return ValidationProblem(new ValidationProblemDetails(errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest
            });
        }
        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            return Problem(
                title: "Conflict",
                detail: result.Error,
                statusCode: StatusCodes.Status409Conflict

            );
        }

        return Accepted(result.Value);
    }

    public record CreateLoanApplicationRequest(
        Guid CustomerId,
        Guid ProductId,
        decimal RequestedAmount,
        int TermMonths,
        string Purpose);
}