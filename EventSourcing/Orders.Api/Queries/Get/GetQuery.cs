using Orders.Domain;

namespace Orders.Api.Queries.Get;

public record GetRequest(string OrderId);

public record GetResponse(Order Order);

public record Order(string Id);

public class GetQuery : IGetQuery
{
    public async Task<GetResponse> Execute(GetRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}