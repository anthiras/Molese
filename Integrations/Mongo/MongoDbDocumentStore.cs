using System.Runtime.CompilerServices;
using Framework;
using MongoDB.Driver;

namespace Mongo;

public class MongoDbDocumentStore(IMongoClient client) : IDocumentStore
{
    public async Task<TDocument> Find<TDocument, TId>(Id<TId> id, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        var result = await Collection<TDocument>().FindAsync(
            Builders<TDocument>.Filter.Eq(doc => doc.Id, id), null, ct);
        return await result.FirstAsync(ct);
    }

    public async IAsyncEnumerable<TDocument> FindAll<TDocument>([EnumeratorCancellation] CancellationToken ct = default)
    {
        var result = await Collection<TDocument>().FindAsync(Builders<TDocument>.Filter.Empty, null, ct);
        while (await result.MoveNextAsync(ct))
            foreach (var doc in result.Current)
                yield return doc;
    }

    public async Task Store<TDocument, TId>(TDocument document, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        await Collection<TDocument>().ReplaceOneAsync(Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id),
            document, new ReplaceOptions() { IsUpsert = true }, ct);
    }

    public async Task Delete<TDocument, TId>(Id<TId> id, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        await Collection<TDocument>().DeleteOneAsync(Builders<TDocument>.Filter.Eq(doc => doc.Id, id), ct);
    }

    private IMongoCollection<TDocument> Collection<TDocument>() =>
        client.GetDatabase("DB").GetCollection<TDocument>(nameof(TDocument));
}