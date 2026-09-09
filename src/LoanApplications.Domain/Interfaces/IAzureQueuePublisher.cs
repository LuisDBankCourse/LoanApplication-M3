namespace LoanApplications.Domain.Interfaces;

public interface IAzureQueuePublisher
{
    Task PublishAsync(string queueName, string message, CancellationToken ct = default);
}