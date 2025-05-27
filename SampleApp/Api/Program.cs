using Framework;
using SampleApp.Api.Aircrafts;
using SampleApp.Application;
using SampleApp.Domain;
using InMemory;
using SampleApp.Api;
using SampleApp.Application.Projections;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
    .RegisterHandlersFromAssemblies(Assemblies.Domain, Assemblies.Application)
    .RegisterAggregateRootRepository<Aircraft>(Aircraft.Create)
    .RegisterAggregateRootRepository<Flight>(Flight.Create)
    .RegisterDocumentRepository<AircraftListItem>()
    .AddEventSubscriber()
    .AddInMemoryEventStore() // Replace with your favorite event store
    .AddInMemoryDocumentStore(); // Replace with your favorite document store

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
