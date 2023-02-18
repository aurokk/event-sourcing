namespace Payments.DataAccess.Read;

public class Offset
{
    public string Id { get; }
    public string? Value { get; private set; }
    public string? Etag { get; private set; }
    public string? SavedEtag { get; private set; }

    private Offset(string id, string? value, string etag, string? savedEtag)
    {
        Id = id;
        Value = value;
        Etag = etag;
        SavedEtag = savedEtag;
    }

    public static Offset Create(string id)
        => new Offset(
            id: id,
            value: null,
            etag: Guid.NewGuid().ToString("N"),
            savedEtag: null);

    public static Offset FromDatabase(string id, string value, string etag)
        => new Offset(
            id: id,
            value: value,
            etag: etag,
            savedEtag: etag);


    public void Update(string value)
    {
        Value = value;
        Etag = Guid.NewGuid().ToString("N");
    }

    public void Commit()
    {
        SavedEtag = Etag;
    }
}