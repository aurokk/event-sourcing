using System.Text.Json;
using EventStore.Client;
using Payments.DataAccess.Read;

namespace Payments.Api.Notifier;

public class NotifierService
{
    private readonly IOffsetsRepository _offsetsRepository;
    private readonly IEventProcessor[] _eventProcessors;
    private readonly EventStoreClient _client;

    private const string Id = "NotifierService";

    public NotifierService(
        IOffsetsRepository offsetsRepository,
        IEnumerable<IEventProcessor> eventProcessors,
        EventStoreClient client)
    {
        _offsetsRepository = offsetsRepository;
        _eventProcessors = eventProcessors.ToArray();
        _client = client;
    }

    private Offset? _offset;

    public async Task Run(CancellationToken ct)
    {
        _offset = await _offsetsRepository.TryGet(Id, ct) ?? Offset.Create(Id);

        await _client.SubscribeToAllAsync(
            start: GetPosition(),
            eventAppeared: EventAppeared,
            cancellationToken: ct
        );
    }

    private async Task EventAppeared(StreamSubscription subscription, ResolvedEvent evt, CancellationToken ct)
    {
        var eventProcessor = _eventProcessors.FirstOrDefault(x => x.CanProcess(evt.Event));
        if (eventProcessor != null)
        {
            await eventProcessor.Process(evt.Event, ct);
        }

        var position = evt.OriginalPosition;
        if (position.HasValue)
        {
            SetPosition(position.Value);
            await _offsetsRepository.Set(_offset!, ct);
        }
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        IncludeFields = true,
        Converters = { new PositionJsonConverter(), },
    };

    private FromAll GetPosition()
    {
        var value = _offset?.Value;
        return value != null
            ? FromAll.After(JsonSerializer.Deserialize<Position>(value, JsonSerializerOptions))
            : FromAll.Start;
    }

    private void SetPosition(Position position)
    {
        var value = JsonSerializer.Serialize(position, JsonSerializerOptions);
        _offset?.Update(value);
    }
}