using LoanApplications.Domain.Enums;

namespace LoanApplications.Domain.Exceptions;

public class InvalidLoanApplicationStateException : Exception
{
    public InvalidLoanApplicationStateException(
        Guid applicationId,
        LoanApplicationStatus currentStatus,
        string attemptedOperation
    ) : base(
        $"the state of the application id {applicationId} is {currentStatus.ToString()} and is not allowed to {attemptedOperation}")

    {

    }
}