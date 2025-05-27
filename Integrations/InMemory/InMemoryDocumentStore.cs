using Framework;

namespace InMemory;

public class InMemoryDocumentStore<TDocument> : IDocumentStore<TDocument> where TDocument : Document<TDocument>
{
    private readonly Dictionary<Id<TDocument>, TDocument> _documents = [];
    
    public Task<TDocument> Find(Id<TDocument> id, CancellationToken ct = default)
    {
        return Task.FromResult(_documents[id]);
    }

    public Task Store(TDocument document, CancellationToken ct = default)
    {
        _documents[document.Id] = document;
        return Task.CompletedTask;
    }

    public Task Delete(Id<TDocument> id, CancellationToken ct = default)
    {
        _documents.Remove(id);
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<TDocument> FindAll(CancellationToken ct = default)
    {
        return _documents.Values.ToAsyncEnumerable();
    }
}