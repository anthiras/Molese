namespace Framework;

public interface IDocumentStore
{
    Task<TDocument> Find<TDocument, TId>(Id<TId> id, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>;
    Task Store<TDocument, TId>(TDocument document, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>;
    
    async Task Update<TDocument, TId>(Id<TId> id, Action<TDocument> updateAction, CancellationToken ct = default) where TDocument : IIdentifiable<Id<TId>>
    {
        var entity = await Find<TDocument, TId>(id, ct);
        updateAction(entity);
        await Store<TDocument, TId>(entity, ct);
    }
}