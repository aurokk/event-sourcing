using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Payments.Contracts;

[PublicAPI]
public class PaymentFulfilledNotification
{
    public string ReferenceId { get; }
    public string PaymentId { get; }

    public PaymentFulfilledNotification(string referenceId, string paymentId)
    {
        ReferenceId = referenceId;
        PaymentId = paymentId;
    }
}