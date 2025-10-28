using Framework;

namespace InMemory;

public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : IIdentifiable<Id<TEntity>>
{
    private readonly Dictionary<Id<TEntity>, TEntity> _entities = new();
    
    public Task<TEntity> Find(Id<TEntity> id, CancellationToken ct = default)
    {
        return Task.FromResult(_entities[id]);
    }

    public Task Store(TEntity entity, CancellationToken ct = default)
    {
        _entities[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task Delete(TEntity entity, CancellationToken ct = default)
    {
        _entities.Remove(entity.Id);
        return Task.CompletedTask;
    }
}