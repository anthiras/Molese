using Framework;
using SampleApp.Api.Aircrafts;
using InMemory;
using SampleApp.Api;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
    .RegisterHandlersFromAssemblies(Assemblies.Domain, Assemblies.Application)
    .RegisterEventSourcedRepositories(Assemblies.Domain)
    .RegisterDocumentRepositories(Assemblies.Application)
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
