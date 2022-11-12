namespace Orders.Api.Queries.Get;

public interface IGetQuery
{
    Task<GetResponse> Execute(GetRequest request, CancellationToken ct);
}