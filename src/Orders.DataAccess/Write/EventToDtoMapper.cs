using System.Text.Json;
using Common.Abstractions;
using EventStore.Client;
using JetBrains.Annotations;

namespace Orders.DataAccess.Write;

[UsedImplicitly]
public class EventToDtoMapper
{
    public static EventData Map(DomainEvent evt) =>
        new(
            Uuid.NewUuid(),
            evt.EventType,
            JsonSerializer.SerializeToUtf8Bytes(evt, evt.GetType())
        );
}