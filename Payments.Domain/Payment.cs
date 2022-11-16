using Common.Abstractions;
using JetBrains.Annotations;

namespace Payments.Domain;

public enum PaymentStatus
{
    Uninitialized = 0,
    Initialized = 1,
    Started = 2,
    Fulfilled = 3,
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Payment : Aggregate
{
    private PaymentStatus Status = PaymentStatus.Uninitialized;

    public Payment()
    {
    }

    public static Payment Create(string id, DateTime utcNow)
    {
        var payment = new Payment();
        var @event = new PaymentCreated(id, utcNow);
        payment.ApplyEvent(@event);
        return payment;
    }

    public void Authorize(DateTime utcNow)
    {
        if (Status != PaymentStatus.Initialized)
        {
            throw new InvalidOperationException($"Payment must be in {nameof(PaymentStatus.Initialized)} status");
        }

        var @event = new PaymentStarted(Id!, utcNow);
        ApplyEvent(@event);
    }

    public void Fulfill(DateTime utcNow)
    {
        if (Status != PaymentStatus.Started)
        {
            throw new InvalidOperationException($"Payment must be in {nameof(PaymentStatus.Started)} status");
        }

        var @event = new PaymentFulfilled(Id!, utcNow);
        ApplyEvent(@event);
    }

    protected void Apply(PaymentCreated @event)
    {
        Id = @event.AggregateId;
        Status = PaymentStatus.Initialized;
    }

    protected void Apply(PaymentStarted @event)
    {
        Status = PaymentStatus.Started;
    }

    protected void Apply(PaymentFulfilled @event)
    {
        Status = PaymentStatus.Fulfilled;
    }
}