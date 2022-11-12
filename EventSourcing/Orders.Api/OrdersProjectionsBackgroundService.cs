using System.Text.Json;
using System.Text.Json.Serialization;
using EventStore.Client;
using Orders.DataAccess.Read;
using Orders.DataAccess.Write;
using Orders.Domain;
using IOrdersRepository = Orders.DataAccess.Read.IOrdersRepository;

namespace Orders.Api;

public class OrdersProjectionsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OrdersProjectionsBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var projectionsService = serviceProvider.GetRequiredService<OrdersProjectionsService>();
        await projectionsService.Run(stoppingToken);
    }
}

public class OrdersProjectionsService
{
    private readonly IOffsetsRepository _offsetsRepository;
    private readonly IEventProcessor[] _eventProcessors;
    private readonly EventStoreClient _client;

    private const string Id = "OrdersProjectionsService";

    public OrdersProjectionsService(
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
        Console.WriteLine(evt.Event.EventType);
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

public interface IEventProcessor
{
    bool CanProcess(EventRecord eventRecord);
    Task Process(EventRecord eventRecord, CancellationToken ct);
}

public class OrderCreatedProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IOrdersRepository _ordersRepository;

    public OrderCreatedProcessor(EventToDomainMapper mapper, IOrdersRepository ordersRepository)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == OrderCreated.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        var domainEvent = _mapper.Map(eventRecord);
        var order = DataAccess.Read.Order.Create(domainEvent.AggregateId);
        await _ordersRepository.Create(order, ct);
    }
}