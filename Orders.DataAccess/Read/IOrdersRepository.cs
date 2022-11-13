namespace Orders.DataAccess.Read;

public interface IOrdersRepository
{
    Task Create(Order order, CancellationToken ct);
    Task Update(Order order, CancellationToken ct);
    Task<Order> Get(string id, CancellationToken ct);
}