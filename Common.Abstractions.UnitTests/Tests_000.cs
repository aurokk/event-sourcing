namespace Common.Abstractions.UnitTests;

public class TestAggregate0Created : DomainEvent
{
    public override string EventType => "TestAggregate0Created";

    public TestAggregate0Created(string aggregateId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
    }
}

public class TestAggregate0 : Aggregate
{
    private TestAggregate0()
    {
    }

    public static TestAggregate0 Create(string id, DateTime utcNow)
    {
        var testAggregate0 = new TestAggregate0();

        var @event = new TestAggregate0Created(
            aggregateId: id,
            createdAtUtc: utcNow
        );

        testAggregate0.ApplyEvent(@event);
        return testAggregate0;
    }

    private void Apply(TestAggregate0Created @event)
    {
        Id = @event.AggregateId;
    }
}

public class Tests_000
{
    private readonly string _id = Guid.NewGuid().ToString("N");
    private readonly DateTime _utcNow = DateTime.UtcNow;

    [Test]
    public void Test_000()
    {
        var sut = TestAggregate0.Create(_id, _utcNow);
        var events = sut.GetEventsToCommit();
        Assert.That(events, Has.Length.EqualTo(1));
        Assert.That(events[0], Is.InstanceOf<TestAggregate0Created>());
    }

    [Test]
    public void Test_001()
    {
        var sut = TestAggregate0.Create(_id, _utcNow);
        sut.GetEventsToCommit();
        sut.ClearEventsToCommit();
        var events = sut.GetEventsToCommit();
        Assert.That(events, Is.Empty);
    }
}