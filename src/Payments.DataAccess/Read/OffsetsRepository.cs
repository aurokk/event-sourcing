using MongoDB.Driver;

namespace Payments.DataAccess.Read;

public class OffsetsRepository : IOffsetsRepository
{
    private readonly IMongoCollection<OffsetDto> _offsetsCollection;

    public OffsetsRepository(IMongoDatabase mongoDatabase) =>
        _offsetsCollection = mongoDatabase.GetCollection<OffsetDto>("offsets");

    public async Task<Offset?> TryGet(string id, CancellationToken ct)
    {
        var offsetDto = await _offsetsCollection
            .Find(x => x.Id == id)
            .SingleOrDefaultAsync(cancellationToken: ct);
        return offsetDto != null
            ? ToDomain(offsetDto)
            : null;
    }

    public async Task Set(Offset offset, CancellationToken ct)
    {
        var offsetDto = ToDto(offset);
        await _offsetsCollection
            .ReplaceOneAsync(
                x => x.Id == offset.Id && x.Etag == offset.SavedEtag,
                offsetDto,
                new ReplaceOptions() { IsUpsert = true, },
                cancellationToken: ct
            );
        offset.Commit();
    }

    private static OffsetDto ToDto(Offset offset) =>
        new OffsetDto
        {
            Id = offset.Id,
            Value = offset.Value,
            Etag = offset.Etag,
        };

    private static Offset ToDomain(OffsetDto offsetDto) =>
        Offset.FromDatabase(
            id: offsetDto.Id ?? throw new ApplicationException(),
            value: offsetDto.Value ?? throw new ApplicationException(),
            etag: offsetDto.Etag ?? throw new ApplicationException()
        );
}