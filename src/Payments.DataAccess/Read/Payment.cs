namespace Payments.DataAccess.Read;

public enum PaymentStatus
{
    Initialized = 1,
    Started = 2,
    Fulfilled = 3,
}

public class Payment
{
    public string Id { get; }
    public PaymentStatus PaymentStatus { get; private set; }
    public string? Etag { get; private set; }
    public string? SavedEtag { get; private set; }

    private Payment(string id, string etag, string? savedEtag, PaymentStatus paymentStatus)
    {
        Id = id;
        Etag = etag;
        SavedEtag = savedEtag;
        PaymentStatus = paymentStatus;
    }

    public static Payment Create(string id) =>
        new Payment(
            id: id,
            etag: Guid.NewGuid().ToString("N"),
            savedEtag: null,
            paymentStatus: PaymentStatus.Initialized
        );

    public static Payment FromDatabase(
        string id,
        string etag,
        PaymentStatus paymentStatus
    ) =>
        new Payment(
            id: id,
            etag: etag,
            savedEtag: etag,
            paymentStatus: paymentStatus
        );

    public void SetStarted()
    {
        PaymentStatus = PaymentStatus.Started;
        Etag = Guid.NewGuid().ToString("N");
    }

    public void SetFulfilled()
    {
        PaymentStatus = PaymentStatus.Fulfilled;
        Etag = Guid.NewGuid().ToString("N");
    }

    public void Commit()
    {
        SavedEtag = Etag;
    }
}