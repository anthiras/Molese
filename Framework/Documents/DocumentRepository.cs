namespace Framework;

public class DocumentRepository<TDocument>(IDocumentStore<TDocument> store) : IRepository<TDocument> where TDocument : Document<TDocument>
{
    public Task<TDocument> Find(Id<TDocument> id, CancellationToken ct = default)
    {
        return store.Find(id, ct);
    }

    public Task Store(TDocument entity, CancellationToken ct = default)
    {
        return store.Store(entity, ct);
    }

    public Task Delete(Id<TDocument> id, CancellationToken ct = default)
    {
        return store.Delete(id, ct);
    }
}