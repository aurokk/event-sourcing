using Common.Abstractions;
using JetBrains.Annotations;

namespace Payments.Domain;

public enum PaymentStatus
{
    Unknown = 0,
    Created = 1,
    Started = 2,
    Completed = 3,
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Payment : Aggregate
{
    private PaymentStatus Status = PaymentStatus.Unknown;
    public string? ReferenceId { get; private set; }

    public Payment()
    {
    }

    public static Payment Create(string id, string referenceId, decimal amount, int currencyCode, DateTime utcNow)
    {
        var payment = new Payment();
        var @event = new PaymentCreated(id, referenceId, amount, currencyCode, utcNow);
        payment.ApplyEvent(@event);
        return payment;
    }

    public void Start(DateTime utcNow)
    {
        if (Status != PaymentStatus.Created)
        {
            throw new InvalidOperationException($"Payment must be in {nameof(PaymentStatus.Created)} status");
        }

        var @event = new PaymentStarted(Id!, utcNow);
        ApplyEvent(@event);
    }

    public void Completed(DateTime utcNow)
    {
        if (Status != PaymentStatus.Started)
        {
            throw new InvalidOperationException($"Payment must be in {nameof(PaymentStatus.Started)} status");
        }

        var @event = new PaymentCompleted(Id!, utcNow);
        ApplyEvent(@event);
    }

    protected void Apply(PaymentCreated @event)
    {
        Id = @event.AggregateId;
        ReferenceId = @event.ReferenceId;
        Status = PaymentStatus.Created;
    }

    protected void Apply(PaymentStarted @event)
    {
        Status = PaymentStatus.Started;
    }

    protected void Apply(PaymentCompleted @event)
    {
        Status = PaymentStatus.Completed;
    }
}