using System.Reflection;

namespace Common.Abstractions;

public abstract class Aggregate
{
    public string? Id { get; protected set; }

    private readonly List<DomainEvent> _eventsToCommit;

    protected Aggregate()
    {
        _eventsToCommit = new List<DomainEvent>();
    }

    public void LoadFromHistory(IEnumerable<DomainEvent> history)
    {
        foreach (var e in history)
        {
            ApplyEvent(e, false);
        }
    }

    protected void ApplyEvent(DomainEvent @event)
    {
        ApplyEvent(@event, true);
    }

    private void ApplyEvent(DomainEvent @event, bool isNew)
    {
        // You can use some library, like ReflectionMagic, to make this code easier
        // Or you can use reflection, cache methods and make the code work faster
        var method = this
            .GetType()
            .GetMethod(
                "Apply",
                BindingFlags.Instance | BindingFlags.NonPublic,
                new[] { @event.GetType(), }
            );

        if (method == null)
        {
            throw new Exception($"There is no applicable method Apply({@event.GetType().Name})");
        }

        method.Invoke(this, new object[] { @event });

        if (isNew)
        {
            _eventsToCommit.Add(@event);
        }
    }

    public DomainEvent[] GetEventsToCommit()
    {
        return _eventsToCommit.ToArray();
    }

    public void ClearEventsToCommit()
    {
        _eventsToCommit.Clear();
    }
}