using FluentValidation;
using LoanApplications.Application.Commands;
using LoanApplications.Application.Commands.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LoanApplications.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class LoanApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateLoanApplicationCommand> _validator;
    private readonly IValidator<UpdateDecisionCommand> _updateValidator;
    private readonly ILogger<LoanApplicationsController> _logger;

    public LoanApplicationsController(IMediator mediator, 
        IValidator<CreateLoanApplicationCommand> validator,
        IValidator<UpdateDecisionCommand> updateValidator, 
        ILogger<LoanApplicationsController> logger)
    {
        _mediator = mediator;
        _validator = validator;
        _updateValidator = updateValidator;
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

    [HttpPost("{id:guid}/decision")]
    [ProducesResponseType(typeof(UpdateDecisionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDecision (Guid id, [FromBody] UpdateDecisionCommand request, CancellationToken ct)
    {

        var validation = await _updateValidator.ValidateAsync(request, ct);

        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g
                    .Select(x => x.ErrorMessage)
                    .ToArray());

            return ValidationProblem(new ValidationProblemDetails(errors)
            {
                Title = "Validación fallida",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var result = await _mediator.Send(request, ct);

        if (!result.IsSuccess)
        {
            return Problem(
                title: "Not found",
                detail: result.Error,
                statusCode: StatusCodes.Status404NotFound

            );
        }

        return Ok(result.Value);

    }



    public record CreateLoanApplicationRequest
    (
        Guid CustomerId,
        Guid ProductId,
        decimal RequestedAmount,
        int TermMonths,
        string Purpose

    );
}