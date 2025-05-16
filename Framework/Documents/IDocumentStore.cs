namespace Framework;

public interface IDocumentStore
{
    Task<TDocument> Find<TDocument, TId>(Id<TId> id, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>;
    IAsyncEnumerable<TDocument> FindAll<TDocument>(CancellationToken ct = default);
    Task Store<TDocument, TId>(TDocument document, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>;
    Task Delete<TId>(Id<TId> id, CancellationToken ct = default);
    
    async Task Update<TDocument, TId>(Id<TId> id, Action<TDocument> updateAction, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        var entity = await Find<TDocument, TId>(id, ct);
        updateAction(entity);
        await Store<TDocument, TId>(entity, ct);
    }
}