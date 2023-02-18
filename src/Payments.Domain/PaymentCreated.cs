using Common.Abstractions;

namespace Payments.Domain;

public class PaymentCreated : DomainEvent
{
    public const string Type = "PaymentCreated";
    public override string EventType => Type;
    public string ReferenceId { get; }
    public decimal Amount { get; }
    public int CurrencyCode { get; }

    public PaymentCreated(
        string aggregateId,
        string referenceId,
        decimal amount,
        int currencyCode,
        DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
        ReferenceId = referenceId;
        Amount = amount;
        CurrencyCode = currencyCode;
    }
}