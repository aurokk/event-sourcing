using MongoDB.Driver;

namespace Payments.DataAccess.Read;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly IMongoCollection<PaymentDto> _paymentsCollection;

    public PaymentsRepository(IMongoDatabase mongoDatabase) =>
        _paymentsCollection = mongoDatabase.GetCollection<PaymentDto>("payments");

    public Task Create(Payment order, CancellationToken ct) =>
        Store(order, ct);

    public Task Update(Payment order, CancellationToken ct) =>
        Store(order, ct);

    private async Task Store(Payment order, CancellationToken ct)
    {
        var paymentDto = ToDto(order);
        await _paymentsCollection
            .ReplaceOneAsync(
                x => x.Id == order.Id && x.Etag == order.SavedEtag,
                paymentDto,
                new ReplaceOptions { IsUpsert = true, },
                cancellationToken: ct
            );
        order.Commit();
    }

    public async Task<Payment> Get(string id, CancellationToken ct)
    {
        var paymentDto = await _paymentsCollection
            .Find(x => x.Id == id)
            .SingleOrDefaultAsync(cancellationToken: ct);
        return paymentDto != null
            ? ToDomain(paymentDto)
            : throw new Exception();
    }

    private static Payment ToDomain(PaymentDto paymentDto) =>
        Payment.FromDatabase(
            id: paymentDto.Id ?? throw new Exception(),
            etag: paymentDto.Etag ?? throw new Exception(),
            paymentStatus: Enum.Parse<PaymentStatus>(paymentDto.PaymentStatus ?? throw new Exception())
        );

    private static PaymentDto ToDto(Payment payment) =>
        new PaymentDto
        {
            Id = payment.Id,
            PaymentStatus = payment.PaymentStatus.ToString("G"),
        };
}