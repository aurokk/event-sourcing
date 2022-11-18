using Raven.Client.Documents;

namespace Payments.DataAccess.Read;

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