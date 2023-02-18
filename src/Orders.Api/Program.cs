using EventStore.Client;
using MassTransit;
using MongoDB.Driver;
using Orders.Api;
using Orders.Api.Commands.AddToCart;
using Orders.Api.Commands.Checkout;
using Orders.Api.Commands.Create;
using Orders.Api.Commands.DeleteFromCart;
using Orders.Api.OrdersProjections;
using Orders.Api.Queries.Get;
using Orders.DataAccess.Write;
using Orders.Domain;
using Orders.PaymentsClient;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddScoped<ICreateCommand, CreateCommand>()
    .AddScoped<IAddToCartCommand, AddToCartCommand>()
    .AddScoped<IDeleteFromCartCommand, DeleteFromCartCommand>()
    .AddScoped<ICheckoutCommand, CheckoutCommand>()
    .AddScoped<IGetQuery, GetQuery>();

services
    .AddScoped<IOrdersRepository, OrdersRepository>();

services
    .AddHttpClient<IPaymentsClient, PaymentsClient>();

services
    .AddScoped<Orders.DataAccess.Read.IOffsetsRepository, Orders.DataAccess.Read.OffsetsRepository>()
    .AddScoped<Orders.DataAccess.Read.IOrdersRepository, Orders.DataAccess.Read.OrdersRepository>();

services
    .AddHostedService<OrdersProjectionsBackgroundService>()
    .AddScoped<OrdersProjectionsService>()
    .AddScoped<IEventProcessor, OrderCreatedProcessor>()
    .AddScoped<IEventProcessor, ItemAddedToCartProcessor>()
    .AddScoped<IEventProcessor, ItemDeletedFromCartProcessor>()
    .AddScoped<IEventProcessor, OrderPlacedProcessor>()
    .AddScoped<IEventProcessor, OrderPaidProcessor>();

services
    .AddScoped<EventToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, OrderCreatedToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, ItemAddedToCartToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, ItemDeletedFromCartToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, OrderPaidToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, OrderPlacedToDomainMapper>();

services
    .AddScoped<IPaymentsClient, PaymentsClient>();

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
            .GetDatabase("orders")
    );

services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentFulfilledNotificationConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(Environment.GetEnvironmentVariable("RabbitMq__ConnectionString"));
        cfg.ReceiveEndpoint("orders", e => e.ConfigureConsumer<PaymentFulfilledNotificationConsumer>(context));
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