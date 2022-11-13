using Orders.DataAccess.Read;

namespace Orders.Api.Queries.Get;

public record GetRequest(string OrderId);

public record GetResponse(Order Order);

public record Order(string Id);

public class GetQuery : IGetQuery
{
    private readonly IOrdersRepository _ordersRepository;

    public GetQuery(IOrdersRepository ordersRepository) =>
        _ordersRepository = ordersRepository;

    public async Task<GetResponse> Execute(GetRequest request, CancellationToken ct)
    {
        var order = await _ordersRepository.Get(request.OrderId, ct);
        return new GetResponse(
            Order: new Order(
                Id: order.Id
            )
        );
    }
}