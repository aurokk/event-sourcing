using System.Text.Json;
using EventStore.Client;
using Orders.DataAccess.Read;

namespace Orders.Api.OrdersProjections;

public class OrdersProjectionsService
{
    private readonly IOffsetsRepository _offsetsRepository;
    private readonly IEventProcessor[] _eventProcessors;
    private readonly EventStoreClient _client;
    private readonly ILogger<OrdersProjectionsService> _logger;

    private const string Id = "OrdersProjectionsService";

    public OrdersProjectionsService(
        IOffsetsRepository offsetsRepository,
        IEnumerable<IEventProcessor> eventProcessors,
        EventStoreClient client,
        ILogger<OrdersProjectionsService> logger)
    {
        _offsetsRepository = offsetsRepository;
        _eventProcessors = eventProcessors.ToArray();
        _client = client;
        _logger = logger;
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
        try
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
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong");
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