namespace LoanApplications.Domain.Entities;

public class OutBoxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; }
    public string Payload { get; set; }
    public string CorrelationId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public string? Error  { get; set; }
    public int RetryCount { get; set; }

    public static OutBoxMessage Create(
        string eventType,
        string payload,
        string correlationId
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        return new OutBoxMessage
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            Payload = payload,
            CorrelationId = correlationId,
            CreatedAtUtc = DateTime.UtcNow,
            RetryCount = 0,
        };
    }
    
    public void MarkAsProcessed()
    {
        ProcessedAtUtc = DateTime.UtcNow;
        Error = string.Empty;
    }

    public void MarkAsFailed(string error)
    {
        RetryCount = RetryCount +1;
        Error = error;
    }
}