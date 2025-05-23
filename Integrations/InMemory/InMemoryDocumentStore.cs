using Framework;

namespace InMemory;

public class InMemoryDocumentStore : IDocumentStore
{
    private readonly Dictionary<string, object> _documents = [];
    
    public Task<TDocument> Find<TDocument, TId>(Id<TId> id, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        return Task.FromResult((TDocument)_documents[id.ToString()]);
    }

    public Task Store<TDocument, TId>(TDocument document, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        _documents[document.Id.ToString()] = document;
        return Task.CompletedTask;
    }

    public Task Delete<TDocument, TId>(Id<TId> id, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        _documents.Remove(id.ToString());
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<TDocument> FindAll<TDocument>(CancellationToken ct = default)
    {
        return _documents.Values.Cast<TDocument>().ToAsyncEnumerable();
    }
}