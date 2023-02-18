namespace Payments.DataAccess.Read;

public class PaymentDto
{
    public string? Id { get; set; }
    public string? PaymentStatus { get; set; }
    public string? Etag { get; set; }
}