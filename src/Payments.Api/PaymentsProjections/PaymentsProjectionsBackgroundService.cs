namespace Payments.Api.PaymentsProjections;

public class PaymentsProjectionsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PaymentsProjectionsBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var svc = serviceProvider.GetRequiredService<PaymentsProjectionsService>();
        await svc.Run(stoppingToken);
    }
}