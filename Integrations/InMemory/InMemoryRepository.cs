using Framework;

namespace InMemory;

public class InMemoryRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : IIdentifiable<TId> where TId : notnull
{
    private readonly Dictionary<TId, TEntity> _entities = new();
    
    public Task<TEntity> Find(TId id, CancellationToken ct = default)
    {
        return Task.FromResult(_entities[id]);
    }

    public Task Store(TEntity entity, CancellationToken ct = default)
    {
        _entities[entity.Id] = entity;
        return Task.CompletedTask;
    }
}