using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Payments.Contracts;

[PublicAPI]
public class PaymentCompletedNotification
{
    public string ReferenceId { get; }
    public string PaymentId { get; }

    public PaymentCompletedNotification(string referenceId, string paymentId)
    {
        ReferenceId = referenceId;
        PaymentId = paymentId;
    }
}