namespace Framework;

public interface IRepository<TEntity>
{
    Task<TEntity> Find(Id<TEntity> id, CancellationToken ct = default);
    Task Store(TEntity entity, CancellationToken ct = default);
    // TODO: Delete

    async Task Update(Id<TEntity> id, Action<TEntity> updateAction, CancellationToken ct = default)
    {
        var entity = await Find(id, ct);
        updateAction(entity);
        await Store(entity, ct);
    }
}