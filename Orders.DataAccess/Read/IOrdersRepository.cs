using Raven.Client.Documents;

namespace Orders.DataAccess.Read;

public interface IOrdersRepository
{
    Task Create(Order order, CancellationToken ct);
    Task Update(Order order, CancellationToken ct);
    Task<Order> Get(string id, CancellationToken ct);
}

public class Offset
{
    public string Id { get; }
    public string? Value { get; private set; }
    public string? ChangeVector { get; private set; }

    private Offset(string id, string? value, string? changeVector)
    {
        Id = id;
        Value = value;
        ChangeVector = changeVector;
    }

    public static Offset Create(string id)
        => new Offset(id, null, null);

    public static Offset FromDatabase(string id, string value, string changeVector)
        => new Offset(id, value, changeVector);


    public void Update(string value) =>
        Value = value;

    public void Commit(string changeVector) =>
        ChangeVector = changeVector;
}

public class OffsetDto
{
    public string? Id { get; set; }
    public string? Value { get; set; }
}

public interface IOffsetsRepository
{
    Task<Offset?> TryGet(string id, CancellationToken ct);
    Task Set(Offset offset, CancellationToken ct);
}

public class OffsetsRepository : IOffsetsRepository
{
    private readonly IDocumentStore _store;

    public OffsetsRepository(IDocumentStore store) => _store = store;

    public async Task<Offset?> TryGet(string id, CancellationToken ct)
    {
        using var session = _store.OpenAsyncSession();
        var offsetDto = await session.LoadAsync<OffsetDto>(id, ct);
        if (offsetDto == null)
        {
            return null;
        }

        var changeVector = session.Advanced.GetChangeVectorFor(offsetDto);
        return ToDomain(offsetDto, changeVector);
    }

    public async Task Set(Offset offset, CancellationToken ct)
    {
        using var session = _store.OpenAsyncSession();
        var offsetDto = ToDto(offset);
        await session.StoreAsync(
            entity: offsetDto,
            id: offsetDto.Id,
            changeVector: offset.ChangeVector,
            token: ct
        );
        var changeVector = session.Advanced.GetChangeVectorFor(offsetDto);
        offset.Commit(changeVector);
        await session.SaveChangesAsync(ct);
    }

    private static OffsetDto ToDto(Offset offset) =>
        new OffsetDto
        {
            Id = offset.Id,
            Value = offset.Value,
        };

    private static Offset ToDomain(OffsetDto offsetDto, string changeVector) =>
        Offset.FromDatabase(
            id: offsetDto.Id ?? throw new Exception(),
            value: offsetDto.Value ?? throw new Exception(),
            changeVector: changeVector
        );
}