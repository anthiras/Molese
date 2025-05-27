namespace Framework;

public interface IDocumentStore<TDocument> where TDocument : Document<TDocument>
{
    Task<TDocument> Find(Id<TDocument> id, CancellationToken ct = default);
    IAsyncEnumerable<TDocument> FindAll(CancellationToken ct = default);
    Task Store(TDocument document, CancellationToken ct = default);
    Task Delete(Id<TDocument> id, CancellationToken ct = default);
    
    async Task Update(Id<TDocument> id, Action<TDocument> updateAction, CancellationToken ct = default)
    {
        var entity = await Find(id, ct);
        updateAction(entity);
        await Store(entity, ct);
    }
}
