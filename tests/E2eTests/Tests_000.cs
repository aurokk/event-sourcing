using LightBDD.Framework.Scenarios;
using LightBDD.NUnit3;

namespace E2eTests;

public class Tests_000 : FeatureFixture
{
    [Scenario, Category("E2E")]
    public async Task Test_000()
    {
        await Runner
            .WithContext<Tests_000Context>()
            .RunScenarioAsync(
                c => c.CreateOrder(),
                c => c.AddItemToCart(),
                c => c.AddItemToCart(),
                c => c.AddItemToCart(),
                c => c.AddItemToCart(),
                c => c.PollOrderUntil(x =>
                    x.Cart != null &&
                    x.Cart.Length == 4),
                c => c.DeleteItemFromCart(),
                c => c.Checkout(),
                c => c.PollOrderUntil(x =>
                    x.PaymentId != null),
                c => c.Authorize(),
                c => c.PollOrderUntil(x =>
                    x.OrderStatus != null &&
                    x.OrderStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            );
    }
}