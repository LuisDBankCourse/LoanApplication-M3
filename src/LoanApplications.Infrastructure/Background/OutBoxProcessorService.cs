using LoanApplications.Domain.Interfaces;
using LoanApplications.Domain.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoanApplications.Infrastructure.Background;

public class OutBoxProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutBoxProcessorService> _logger;

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
    
    private const int BatchSize = 10;
    
    private const string QueueName = "loan-application-queue";
    
    public OutBoxProcessorService(IServiceScopeFactory serviceScopeFactory, ILogger<OutBoxProcessorService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutBoxProcessorService started. Execute each {interval} seconds",  _interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,"An exception occurred in OutBox Processor Service.");
            }
            await Task.Delay(_interval, stoppingToken);
        }
        
    }

    public async Task ProcessPendingMessagesAsync(CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        
        var outBoxRepo = scope.ServiceProvider.GetRequiredService<IOutBoxRepository>();
        var queuePublish = scope.ServiceProvider.GetRequiredService<IAzureQueuePublisher>();

        var pending = await outBoxRepo.GetPendingAsync(BatchSize, ct);

        if (!pending.Any())
            return;
        
        _logger.LogInformation("pending messages already start, quantity:{count} ", pending.Count);

        foreach (var message in pending)
        {
            try
            {
                await queuePublish.PublishAsync(QueueName, message.Payload, ct);
                message.MarkAsProcessed();
            
                _logger.LogInformation("Outbox message posted. MessageId:{MessageId}, EvenType = {EventType}, CorrelationId = {CorrelationId}", 
                    message.Id, message.EventType, message.CorrelationId);
            }
            catch (Exception e)
            {
                message.MarkAsFailed(e.Message);
                
                _logger.LogError(e,"Outbox message failed. MessageId:{MessageId}, RetryCount = {RetryCount}", message.Id, message.RetryCount);
                throw;
            }
        }

        await outBoxRepo.SaveChangesAsync(ct);

    }
    
    
}