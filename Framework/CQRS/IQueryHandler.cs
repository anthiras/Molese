namespace Framework;

public interface IQueryHandler<in TQuery, out TResult>
{
    TResult Query(TQuery query, CancellationToken ct = default);
}