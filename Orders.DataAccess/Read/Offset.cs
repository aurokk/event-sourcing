namespace Orders.DataAccess.Read;

public class Offset
{
    public string Id { get; }
    public string? Value { get; private set; }
    public string? ChangeVector { get; private set; }

    private Offset(string id, string? value, string? changeVector)
    {
        Id = id;
        Value = value;
        ChangeVector = changeVector;
    }

    public static Offset Create(string id)
        => new Offset(id, null, null);

    public static Offset FromDatabase(string id, string value, string changeVector)
        => new Offset(id, value, changeVector);


    public void Update(string value) =>
        Value = value;

    public void Commit(string changeVector) =>
        ChangeVector = changeVector;
}