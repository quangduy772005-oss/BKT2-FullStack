namespace PCM.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    // Repository getters
    IRepository<T> GetRepository<T>() where T : class;
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}

public interface ISignalRService
{
    Task SendToAllAsync(string method, params object?[] args);
    Task SendToUserAsync(string userId, string method, params object?[] args);
    Task SendToGroupAsync(string groupName, string method, params object?[] args);
    Task AddToGroupAsync(string connectionId, string groupName);
    Task RemoveFromGroupAsync(string connectionId, string groupName);
}

public interface IEmailService
{
    Task SendEmailAsync(string recipient, string subject, string htmlContent);
    Task SendPasswordResetEmailAsync(string email, string resetLink);
}

public interface IBackgroundJobService
{
    void EnqueueJob<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall);
    void ScheduleJob<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall, TimeSpan delay);
}
