using Orders.DataAccess.Read;

namespace Orders.Api.Queries.Get;

public record GetRequest(string OrderId);

public record GetResponse(Order Order);

public record Order(string Id);

public class GetQuery : IGetQuery
{
    private readonly IOffsetsRepository _offsetsRepository;

    public GetQuery(IOffsetsRepository offsetsRepository) => _offsetsRepository = offsetsRepository;

    public async Task<GetResponse> Execute(GetRequest request, CancellationToken ct)
    {
        var offset = await _offsetsRepository.TryGet("dasd", ct);

        if (offset == null)
        {
            offset = Offset.Create("1");
        }

        await _offsetsRepository.Set(offset, ct);

        throw new NotImplementedException();
    }
}