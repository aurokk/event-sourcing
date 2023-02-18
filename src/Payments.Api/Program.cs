using EventStore.Client;
using MassTransit;
using MongoDB.Driver;
using Payments.Api.Authorizer;
using Payments.Api.Commands.Authorize;
using Payments.Api.Commands.Create;
using Payments.Api.Notifier;
using Payments.Api.PaymentsProjections;
using Payments.Api.Queries.Get;
using Payments.DataAccess.Write;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddScoped<IAuthorizeCommand, AuthorizeCommand>()
    .AddScoped<ICreateCommand, CreateCommand>()
    .AddScoped<IGetQuery, GetQuery>();

services
    .AddScoped<Payments.DataAccess.Read.IOffsetsRepository, Payments.DataAccess.Read.OffsetsRepository>()
    .AddScoped<Payments.DataAccess.Read.IPaymentsRepository, Payments.DataAccess.Read.PaymentsRepository>();

services
    .AddScoped<Payments.Domain.IPaymentsRepository, Payments.DataAccess.Write.PaymentsRepository>()
    .AddScoped<EventToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, PaymentCreatedToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, PaymentStartedToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, PaymentFulfilledToDomainMapper>();

services
    .AddHostedService<PaymentsProjectionsBackgroundService>()
    .AddScoped<PaymentsProjectionsService>()
    .AddScoped<Payments.Api.PaymentsProjections.IEventProcessor, PaymentCreatedProcessor>()
    .AddScoped<Payments.Api.PaymentsProjections.IEventProcessor, PaymentFulfilledProcessor>()
    .AddScoped<Payments.Api.PaymentsProjections.IEventProcessor, PaymentStartedProcessor>();


services
    .AddHostedService<AuthorizerBackgroundService>()
    .AddScoped<AuthorizerService>()
    .AddScoped<Payments.Api.Authorizer.IEventProcessor, AuthorizerProcessor>();


services
    .AddHostedService<NotifierBackgroundService>()
    .AddScoped<NotifierService>()
    .AddScoped<Payments.Api.Notifier.IEventProcessor, NotifierProcessor>();

services
    .AddSingleton<EventStoreClient>(_ =>
    {
        // event store
        var settings = EventStoreClientSettings.Create(Environment.GetEnvironmentVariable("WriteDb__ConnectionString"));
        var client = new EventStoreClient(settings);
        return client;
    });

services
    .AddSingleton<IMongoDatabase>(_ =>
        new MongoClient(Environment.GetEnvironmentVariable("ReadDb__ConnectionString"))
            .GetDatabase("payments")
    );

services.AddMassTransit(x =>
    x.UsingRabbitMq((_, cfg) =>
        cfg.Host(Environment.GetEnvironmentVariable("RabbitMq__ConnectionString"))
    )
);

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