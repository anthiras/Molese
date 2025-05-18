using Framework;
using SampleApp.Api.Aircrafts;
using SampleApp.Application;
using SampleApp.Domain;
using InMemory;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
    .AddDomain()
    .AddApplication()
    .AddInMemoryEventStore() // Replace with your favorite event store
    .AddInMemoryDocumentStore() // Replace with your favorite document store
    .AddSingleton<EventSubscriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapAircraftRoutes();

await app.Services.GetRequiredService<EventSubscriber>().Subscribe();
app.Run();
