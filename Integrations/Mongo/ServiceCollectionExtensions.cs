using System.ComponentModel.Design;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Mongo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbDocumentStore(this IServiceCollection services, MongoClientSettings settings)
    {
        return services.AddSingleton<IMongoClient>(new MongoClient(settings))
            .AddSingleton(typeof(IDocumentStore<>), typeof(MongoDbDocumentStore<>));
    }
}