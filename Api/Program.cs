using Api.Aircrafts;
using Application;
using Application.Projections;
using Domain;
using InMemory;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
    .AddDomain()
    .AddApplication()
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

await app.Services.GetRequiredService<ProjectionSubscriber>().Subscribe();
app.Run();
