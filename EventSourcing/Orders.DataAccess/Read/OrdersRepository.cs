using System.Text.Json;

namespace Orders.DataAccess.Read;

public class OrdersRepository : IOrdersRepository
{
    // private readonly ISession _session;
    //
    // public OrderRepository(ISession session) => _session = session;

    public async Task Create(Order order, CancellationToken ct)
    {
        // var statement = await _session.PrepareAsync("INSERT INTO orders (Id, Data) VALUES (?, ?)");
        // var data = JsonSerializer.Serialize(order);
        // await _session.ExecuteAsync(statement.Bind(order.Id, data));
    }

    public async Task Update(Order order, CancellationToken ct)
    {
        // var statement = await _session.PrepareAsync("UPDATE orders SET Data=? WHERE Id=?");
        // var data = JsonSerializer.Serialize(order);
        // await _session.ExecuteAsync(statement.Bind(order.Id, data));
    }

    public async Task<Order> Get(string id, CancellationToken ct)
    {
        // var statement = await _session.PrepareAsync("SELECT Id, Data FROM orders WHERE Id=?");
        // var results = await _session.ExecuteAsync(statement.Bind(id));
        // var orderDb = results.SingleOrDefault();
        // if (orderDb == null)
        // {
        //     throw new Exception();
        // }
        //
        // return Order.FromDatabase(
        //     id: orderDb.GetValue<string>("Id")
        // );

        throw new NotImplementedException();
    }
}