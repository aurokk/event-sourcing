using EventStore.Client;
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
using Raven.Client.Documents;

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
        var settings = EventStoreClientSettings.Create("esdb://admin:changeit@127.0.0.1:2113?tls=false");
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
                "http://127.0.0.1:8080",
            },
            Database = "read",
            Conventions = { },
        };
        store.Initialize();
        return store;
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