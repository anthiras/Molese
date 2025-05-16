namespace Framework;

public interface IRepository<TEntity, in TId> where TEntity : IIdentifiable<TId>
{
    Task<TEntity> Find(TId id, CancellationToken ct = default);
    Task Store(TEntity entity, CancellationToken ct = default);
    // TODO: Delete

    async Task Update(TId id, Action<TEntity> updateAction, CancellationToken ct = default)
    {
        var entity = await Find(id, ct);
        updateAction(entity);
        await Store(entity, ct);
    }
}