namespace Orders.DataAccess.Read;

public interface IOffsetsRepository
{
    Task<Offset?> TryGet(string id, CancellationToken ct);
    Task Set(Offset offset, CancellationToken ct);
}