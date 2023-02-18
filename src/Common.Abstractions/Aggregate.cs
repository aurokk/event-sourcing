using System.Reflection;

namespace Common.Abstractions;

public abstract class Aggregate
{
    public string? Id { get; protected set; }
    private readonly List<DomainEvent> _eventsToCommit;
    private readonly Dictionary<Type, Action<DomainEvent>> _appliers;

    protected Aggregate()
    {
        _eventsToCommit = new List<DomainEvent>();
        _appliers = this
            .GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(x => x.Name.Equals("Apply", StringComparison.Ordinal))
            .ToDictionary<MethodInfo, Type, Action<DomainEvent>>(
                x => x.GetParameters().Single().ParameterType,
                x => @event => x.Invoke(this, new object[] { @event })
            );
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
        if (!_appliers.TryGetValue(@event.GetType(), out var method))
        {
            throw new Exception($"There is no applicable method Apply({@event.GetType().Name})");
        }

        method(@event);

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