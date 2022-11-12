namespace Common.Abstractions;

public abstract class DomainEvent
{
    public abstract string EventType { get; }

    public string AggregateId { get; }
    public DateTime CreatedAtUtc { get; }

    protected DomainEvent(string aggregateId, DateTime createdAtUtc)
    {
        AggregateId = aggregateId;
        CreatedAtUtc = createdAtUtc;
    }
}