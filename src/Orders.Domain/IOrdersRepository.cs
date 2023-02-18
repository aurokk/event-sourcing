namespace Orders.Domain;

public interface IOrdersRepository
{
    Task<Order> Get(string id, CancellationToken ct);
    Task Save(Order order, CancellationToken ct);
}