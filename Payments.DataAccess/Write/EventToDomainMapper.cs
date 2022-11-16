using Common.Abstractions;
using EventStore.Client;
using JetBrains.Annotations;

namespace Payments.DataAccess.Write;

[UsedImplicitly]
public class EventToDomainMapper
{
    private readonly ISpecificEventToDomainMapper[] _mappers;

    public EventToDomainMapper(IEnumerable<ISpecificEventToDomainMapper> mappers) =>
        _mappers = mappers.ToArray();

    public DomainEvent Map(EventRecord evt)
    {
        var mapper = _mappers.FirstOrDefault(x => x.CanMap(evt));
        if (mapper == null)
        {
            throw new Exception();
        }

        return mapper.Map(evt);
    }
}