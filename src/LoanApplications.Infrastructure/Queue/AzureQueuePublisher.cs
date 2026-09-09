using Azure.Messaging.ServiceBus;
using LoanApplications.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LoanApplications.Infrastructure.Queue;

public class AzureQueuePublisher : IAzureQueuePublisher
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<AzureQueuePublisher> _logger;

    public AzureQueuePublisher(ServiceBusClient serviceBusClient, ILogger<AzureQueuePublisher> logger)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    public async Task PublishAsync(string queueName, string message, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(queueName, nameof(queueName));
        ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
        
        await using ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);

        var serviceBusMessage = new ServiceBusMessage(message)
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
        };

        try
        {
            await sender.SendMessageAsync(serviceBusMessage, ct);
            _logger.LogInformation($"Sent message: {serviceBusMessage.MessageId} {queueName}");
        }
        catch (ServiceBusException ex)
        {
            _logger.LogError(ex, $"Failed to send message: {queueName} {serviceBusMessage.MessageId}");
            throw;
        }
    }
    
}