using Cassandra;
using EventStore.Client;
using Orders.Api.Commands.AddToCart;
using Orders.Api.Commands.Checkout;
using Orders.Api.Commands.Create;
using Orders.Api.Commands.DeleteFromCart;
using Orders.Api.Queries.Get;
using Orders.DataAccess;
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
    .AddScoped<EventToDomainMapper>()
    .AddScoped<ISpecificEventToDomainMapper, OrderCreatedToDomainMapper>();

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
    .AddSingleton(_ =>
        // cassandra
        Cluster
            .Builder()
            .AddContactPoint("127.0.0.1:9042")
            .WithDefaultKeyspace("es")
            .Build()
            .Connect()
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