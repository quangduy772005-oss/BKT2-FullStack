using Microsoft.EntityFrameworkCore.Storage;
using PCM.Core.Interfaces;

namespace PCM.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<string, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken)!;
            return true;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        var key = typeof(T).Name;

        if (!_repositories.ContainsKey(key))
        {
            var repositoryType = typeof(EfRepository<>).MakeGenericType(typeof(T));
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(key, repositoryInstance!);
        }

        return (IRepository<T>)_repositories[key];
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        await _context.DisposeAsync();
    }
}
