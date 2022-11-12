namespace Orders.Domain;

public record CreatePaymentRequest(string ReferenceId, decimal Amount, int CurrencyCode);