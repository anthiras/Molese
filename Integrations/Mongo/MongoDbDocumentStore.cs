using System.Runtime.CompilerServices;
using Framework;
using MongoDB.Driver;

namespace Mongo;

public class MongoDbDocumentStore<TDocument>(IMongoClient client) : IDocumentStore<TDocument> where TDocument : Document<TDocument>
{
    public async Task<TDocument> Find(Id<TDocument> id, CancellationToken ct = default)
    {
        var result = await Collection().FindAsync(
            Builders<TDocument>.Filter.Eq(doc => doc.Id, id), null, ct);
        return await result.FirstAsync(ct);
    }

    public async IAsyncEnumerable<TDocument> FindAll([EnumeratorCancellation] CancellationToken ct = default)
    {
        var result = await Collection().FindAsync(Builders<TDocument>.Filter.Empty, null, ct);
        while (await result.MoveNextAsync(ct))
            foreach (var doc in result.Current)
                yield return doc;
    }

    public async Task Store(TDocument document, CancellationToken ct = default)
    {
        await Collection().ReplaceOneAsync(Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id),
            document, new ReplaceOptions() { IsUpsert = true }, ct);
    }

    public async Task Delete(Id<TDocument> id, CancellationToken ct = default)
    {
        await Collection().DeleteOneAsync(Builders<TDocument>.Filter.Eq(doc => doc.Id, id), ct);
    }

    private IMongoCollection<TDocument> Collection() =>
        client.GetDatabase("DB").GetCollection<TDocument>(nameof(TDocument));
}