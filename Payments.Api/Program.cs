using EventStore.Client;
using MassTransit;
using Payments.Api.Authorizer;
using Payments.Api.Commands.Authorize;
using Payments.Api.Commands.Create;
using Payments.DataAccess.Read;
using Payments.DataAccess.Write;
using Payments.Domain;
using Raven.Client.Documents;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddScoped<IAuthorizeCommand, AuthorizeCommand>()
    .AddScoped<ICreateCommand, CreateCommand>();

services
    .AddScoped<IPaymentsRepository, PaymentsRepository>()
    .AddScoped<EventToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, PaymentCreatedToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, PaymentStartedToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, PaymentFulfilledToDomainMapper>();

services
    .AddHostedService<AuthorizerBackgroundService>()
    .AddScoped<IOffsetsRepository, OffsetsRepository>()
    .AddScoped<AuthorizerService>()
    .AddScoped<IEventProcessor, AuthorizerProcessor>()
    .AddScoped<IEventProcessor, NotifierProcessor>();

services
    .AddSingleton<EventStoreClient>(_ =>
    {
        // event store
        var settings = EventStoreClientSettings.Create("esdb://admin:changeit@127.0.0.1:2123?tls=false");
        var client = new EventStoreClient(settings);
        return client;
    });

services
    .AddSingleton<IDocumentStore>(_ =>
    {
        // raven
        var store = new DocumentStore
        {
            Urls = new[]
            {
                "http://127.0.0.1:8081",
            },
            Database = "read",
            Conventions = { },
        };
        store.Initialize();
        return store;
    });

services.AddMassTransit(x =>
{
    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("user");
            h.Password("password");
        });
    });
});

services
    .AddSwaggerGen();

services
    .AddControllers();


var app = builder.Build();

app
    .UseSwagger()
    .UseSwaggerUI();

app
    .MapControllers();

app.Run();