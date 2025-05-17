using JetBrains.Annotations;
using Testcontainers.EventStoreDb;
using Testcontainers.MongoDb;
using Testcontainers.Xunit;
using Xunit.Abstractions;

namespace Test.Kurrent;

[UsedImplicitly]
public class KurrentContainerFixture(IMessageSink messageSink)
    : ContainerFixture<EventStoreDbBuilder, EventStoreDbContainer>(messageSink)
{
    protected override EventStoreDbBuilder Configure(EventStoreDbBuilder builder)
    {
        return builder.WithImage("eventstore/eventstore:22.10.1-buster-slim");
    }
}

[UsedImplicitly]
public class MongoDbContainerFixture(IMessageSink messageSink)
    : ContainerFixture<MongoDbBuilder, MongoDbContainer>(messageSink)
{
    protected override MongoDbBuilder Configure(MongoDbBuilder builder)
    {
        return builder.WithImage("mongo:6.0");
    }
}