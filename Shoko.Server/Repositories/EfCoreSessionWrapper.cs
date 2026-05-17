using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Transaction;
using Shoko.Server.Data;
using Shoko.Server.Repositories.NHibernate;

#nullable enable

namespace Shoko.Server.Repositories;

/// <summary>
/// Thin adapter over <see cref="ShokoDbContext"/> implementing <see cref="ISessionWrapper"/>.
///
/// Provides a migration path for repositories that currently depend on NHibernate's ISession
/// by wrapping EF Core operations behind the same interface. Methods specific to NHibernate's
/// query API (Criteria, QueryOver, HQL) throw NotImplementedException — those repositories
/// must be migrated to LINQ/EF Core directly.
/// </summary>
public class EfCoreSessionWrapper : ISessionWrapper
{
    private readonly ShokoDbContext _context;
    private readonly bool _ownsContext;
    private readonly IServiceScope? _ownedScope;
    private IDbConnection? _connection;

    /// <summary>
    /// Gets the underlying DbContext for direct EF Core operations.
    /// </summary>
    public ShokoDbContext Context => _context;

    public EfCoreSessionWrapper(ShokoDbContext context, bool ownsContext = false)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _ownsContext = ownsContext;
    }

    public EfCoreSessionWrapper(ShokoDbContext context, IServiceScope ownedScope)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _ownedScope = ownedScope ?? throw new ArgumentNullException(nameof(ownedScope));
        _ownsContext = false;
    }

    public ICriteria CreateCriteria<T>() where T : class
    {
        throw new NotImplementedException("NHibernate Criteria API not available in EF Core. Migrate to LINQ.");
    }

    public ICriteria CreateCriteria(Type type)
    {
        throw new NotImplementedException("NHibernate Criteria API not available in EF Core. Migrate to LINQ.");
    }

    public IQuery CreateQuery(string query)
    {
        throw new NotImplementedException("NHibernate HQL not available in EF Core. Migrate to LINQ or UseSqlQuery.");
    }

    public ISQLQuery CreateSQLQuery(string query)
    {
        throw new NotImplementedException("NHibernate SQLQuery not available in EF Core. Migrate to FromSqlRaw/ExecuteSqlRaw.");
    }

    public IQueryOver<T, T> QueryOver<T>() where T : class
    {
        throw new NotImplementedException("NHibernate QueryOver API not available in EF Core. Migrate to LINQ.");
    }

    public IQueryable<T> Query<T>() where T : class
    {
        return _context.Set<T>().AsQueryable();
    }

    TObj ISessionWrapper.Get<TObj>(object id)
    {
        var result = _context.Find(typeof(TObj), id);
        return result is null ? default! : (TObj)result;
    }

    async Task<TObj> ISessionWrapper.GetAsync<TObj>(object id)
    {
        var result = await _context.FindAsync(typeof(TObj), new object?[] { id }).ConfigureAwait(false);
        return result is null ? default! : (TObj)result;
    }

    public ITransaction BeginTransaction()
    {
        var tx = _context.Database.BeginTransaction();
        return new EfCoreTransactionWrapper(tx);
    }

    public void Insert(object entity)
    {
        _context.Add(entity);
        _context.SaveChanges();
    }

    public void SaveOrUpdate(object entity)
    {
        var entry = _context.Entry(entity);
        if (entry.IsKeySet)
        {
            _context.Update(entity);
        }
        else
        {
            _context.Add(entity);
        }

        _context.SaveChanges();
    }

    public async Task InsertAsync(object entity)
    {
        await _context.AddAsync(entity).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task SaveOrUpdateAsync(object entity)
    {
        var entry = _context.Entry(entity);
        if (entry.IsKeySet)
        {
            _context.Update(entity);
        }
        else
        {
            await _context.AddAsync(entity).ConfigureAwait(false);
        }

        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public void Update(object entity)
    {
        _context.Update(entity);
        _context.SaveChanges();
    }

    public async Task UpdateAsync(object entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public void Delete(object entity)
    {
        _context.Remove(entity);
        _context.SaveChanges();
    }

    public async Task DeleteAsync(object entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public IDbConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                var dbConnection = _context.Database.GetDbConnection();
                var wasClosed = dbConnection.State == ConnectionState.Closed;
                if (wasClosed)
                    dbConnection.Open();
                _connection = new OwnedDbConnectionWrapper(dbConnection, wasClosed);
            }
            return _connection;
        }
    }

    private class OwnedDbConnectionWrapper : IDbConnection
    {
        private readonly IDbConnection _inner;
        private readonly bool _wasClosed;

        public OwnedDbConnectionWrapper(IDbConnection inner, bool wasClosed)
        {
            _inner = inner;
            _wasClosed = wasClosed;
        }

        public string ConnectionString { get => _inner.ConnectionString; set => _inner.ConnectionString = value; }
        public ConnectionState State => _inner.State;
        public int ConnectionTimeout => _inner.ConnectionTimeout;
        public string Database => _inner.Database;

        public void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);
        public void Close() { if (!_wasClosed) _inner.Close(); }
        public IDbCommand CreateCommand() => _inner.CreateCommand();
        public void Open() { if (_inner.State == ConnectionState.Closed) _inner.Open(); }

        public IDbTransaction BeginTransaction() => throw new NotImplementedException();
        public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotImplementedException();

        public void Dispose()
        {
            // Do not close the underlying connection — it's owned by DbContext
        }
    }

    private class EfCoreTransactionWrapper : ITransaction
    {
        private readonly IDbContextTransaction _tx;
        private bool _committed;
        private bool _rolledBack;

        public EfCoreTransactionWrapper(IDbContextTransaction tx)
        {
            _tx = tx;
        }

        public bool WasCommitted => _committed;
        public bool WasRolledBack => _rolledBack;
        public bool IsActive => !_committed && !_rolledBack;

        public void Commit()
        {
            _tx.Commit();
            _committed = true;
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            await _tx.CommitAsync(ct).ConfigureAwait(false);
            _committed = true;
        }

        public void Rollback()
        {
            _tx.Rollback();
            _rolledBack = true;
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            await _tx.RollbackAsync(ct).ConfigureAwait(false);
            _rolledBack = true;
        }

        public void Begin() => throw new NotImplementedException("EF Core transactions are auto-committed on Commit/CommitAsync.");
        public void Begin(IsolationLevel isolationLevel) => throw new NotImplementedException("EF Core transactions are auto-committed on Commit/CommitAsync.");
        public void Enlist(DbCommand command) => throw new NotImplementedException("EF Core does not support enlistment in ambient transactions via DbCommand.");
        public void RegisterSynchronization(ISynchronization sync) => throw new NotImplementedException("EF Core does not support transaction synchronization callbacks.");

        public void Dispose() => _tx.Dispose();
    }

    public void Dispose()
    {
        if (_connection is OwnedDbConnectionWrapper)
        {
            _connection = null;
        }

        if (_ownedScope != null)
        {
            _ownedScope.Dispose();
            return;
        }

        if (_ownsContext)
        {
            _context.Dispose();
        }
    }
}
