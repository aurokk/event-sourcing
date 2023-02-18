namespace Orders.Api.OrdersProjections;

public class OrdersProjectionsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OrdersProjectionsBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var svc = serviceProvider.GetRequiredService<OrdersProjectionsService>();
        await svc.Run(stoppingToken);
    }
}