using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NutzCode.InMemoryIndex;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Exceptions;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Services;
using Shoko.Server.Utilities;


namespace Shoko.Server.Repositories;

// ReSharper disable once InconsistentNaming
public abstract class BaseCachedRepository<T, S> : BaseRepository, ICachedRepository, IRepository<T, S>
    where T : class, new()
{
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

    protected readonly DatabaseFactory _databaseFactory;

    private SystemService _systemService;

    // A hack to not have to pass the system service to every cached repository.
    protected SystemService SystemService => _systemService ??= Utils.ServiceContainer.GetRequiredService<SystemService>();

    public PocoCache<S, T> Cache;

    public Action<T> BeginDeleteCallback { get; set; }

    public Action<ISessionWrapper, T> DeleteWithOpenTransactionCallback { get; set; }

    public Action<T> EndDeleteCallback { get; set; }

    public Action<T> BeginSaveCallback { get; set; }

    public Action<ISessionWrapper, T> SaveWithOpenTransactionCallback { get; set; }

    public Action<T> EndSaveCallback { get; set; }

    protected BaseCachedRepository(DatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    protected ShokoDbContext GetDbContext()
    {
        var connectionString = _databaseFactory.Instance.GetConnectionString();
        var provider = EFCoreOptionsExtensions.FromDatabaseType(Utils.SettingsProvider.GetSettings().Database.Type);
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(provider, connectionString);
        return new ShokoDbContext(optionsBuilder.Options);
    }

    public virtual void Populate(ISessionWrapper session, bool displayName = true, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (displayName)
        {
            SystemService.StartupMessage = $"Database Cache - Caching  - {typeof(T).Name}...";
        }

        // This is only called from main thread, so we don't need to lock
        var settings = Utils.SettingsProvider.GetSettings();
        Cache = new PocoCache<S, T>(session.CreateCriteria(typeof(T)).SetTimeout(settings.CachingDatabaseTimeout).List<T>(), SelectKey);

        PopulateIndexes();
    }

    public virtual void Populate(bool displayName = true, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        using var session = _databaseFactory.SessionFactory.OpenSession();
        using var wrappedSession = session.Wrap();
        Populate(wrappedSession, displayName, cancellationToken);
    }

    public void ClearCache()
    {
        WriteLock(Cache.Clear);
    }

    // ReSharper disable once InconsistentNaming
    public virtual T GetByID(S id)
    {
        if (Equals(default(S), id)) throw new InvalidStateException($"Trying to lookup a {typeof(T).Name} by an ID of 0");
        return ReadLock(() => GetByIDUnsafe(id));
    }

    public T GetByID(ISession session, S id)
    {
        return GetByID(id);
    }

    public T GetByID(ISessionWrapper session, S id)
    {
        return GetByID(id);
    }

    public virtual IReadOnlyList<T> GetAll()
    {
        return ReadLock(GetAllUnsafe);
    }

    public IReadOnlyList<T> GetAll(int maxLimit)
    {
        return ReadLock(() => GetAllUnsafe(maxLimit));
    }

    public IReadOnlyList<T> GetAll(ISession session)
    {
        return GetAll();
    }

    public IReadOnlyList<T> GetAll(ISessionWrapper session)
    {
        return GetAll();
    }

    public virtual void Delete(S id)
    {
        Delete(GetByID(id));
    }

    public virtual void Delete(T cr)
    {
        if (cr == null)
        {
            return;
        }

        BeginDeleteCallback?.Invoke(cr);
        Lock(() => DeleteFromDatabaseUnsafe(cr));

        DeleteFromCache(cr);
        EndDeleteCallback?.Invoke(cr);
    }

    protected void DeleteFromCache(T cr)
    {
        WriteLock(() => DeleteFromCacheUnsafe(cr));
    }

    protected void UpdateCache(T cr)
    {
        WriteLock(() => UpdateCacheUnsafe(cr));
    }

    public virtual void Delete(IReadOnlyCollection<T> objs)
    {
        if (objs.Count == 0)
        {
            return;
        }

        foreach (var cr in objs)
        {
            BeginDeleteCallback?.Invoke(cr);
        }

        Lock(() => DeleteFromDatabaseUnsafe(objs));

        WriteLock(
            () =>
            {
                foreach (var cr in objs)
                {
                    DeleteFromCacheUnsafe(cr);
                }
            }
        );

        foreach (var cr in objs)
        {
            EndDeleteCallback?.Invoke(cr);
        }
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public virtual void DeleteWithOpenTransaction(ISessionWrapper session, T cr)
    {
        if (cr == null)
        {
            return;
        }

        DeleteWithOpenTransactionCallback?.Invoke(session, cr);
        Lock(() => session.Delete(cr));

        WriteLock(() => DeleteFromCacheUnsafe(cr));
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public virtual void DeleteWithOpenTransaction(ISession session, T cr)
    {
        if (cr == null)
        {
            return;
        }

        DeleteWithOpenTransactionCallback?.Invoke(session.Wrap(), cr);
        Lock(() => session.Delete(cr));

        WriteLock(() => DeleteFromCacheUnsafe(cr));
    }

    public virtual async Task DeleteWithOpenTransactionAsync(ISession session, T cr)
    {
        if (cr == null) return;

        DeleteWithOpenTransactionCallback?.Invoke(session.Wrap(), cr);
        await Lock(async () => await session.DeleteAsync(cr));

        WriteLock(() => DeleteFromCacheUnsafe(cr));
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public void DeleteWithOpenTransaction(ISession session, IReadOnlyList<T> objs)
    {
        if (objs.Count == 0) return;

        foreach (var cr in objs)
        {
            DeleteWithOpenTransactionCallback?.Invoke(session.Wrap(), cr);
            Lock(() => session.Delete(cr));
        }

        WriteLock(() =>
        {
            foreach (var obj in objs)
                DeleteFromCacheUnsafe(obj);
        });
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public void DeleteWithOpenTransaction(ISessionWrapper session, IReadOnlyList<T> objs)
    {
        if (objs.Count == 0) return;

        foreach (var cr in objs)
        {
            DeleteWithOpenTransactionCallback?.Invoke(session, cr);
            Lock(() => session.Delete(cr));
        }

        WriteLock(() =>
        {
            foreach (var obj in objs)
                DeleteFromCacheUnsafe(obj);
        });
    }

    public async Task DeleteWithOpenTransactionAsync(ISession session, IReadOnlyList<T> objs)
    {
        if (objs.Count == 0) return;

        foreach (var cr in objs)
        {
            DeleteWithOpenTransactionCallback?.Invoke(session.Wrap(), cr);
            await Lock(async () => await session.DeleteAsync(cr));
        }

        WriteLock(() =>
        {
            foreach (var obj in objs)
                DeleteFromCacheUnsafe(obj);
        });
    }

    public virtual void Save(T obj)
    {
        BeginSaveCallback?.Invoke(obj);
        BaseRepository.Lock(() =>
        {
            using var context = GetDbContext();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var entry = context.Entry(obj);
                if (entry.IsKeySet)
                {
                    context.Update(obj);
                }
                else
                {
                    context.Add(obj);
                }
                context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });

        using var session = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
        SaveWithOpenTransactionCallback?.Invoke(session, obj);

        WriteLock(() => UpdateCacheUnsafe(obj));

        EndSaveCallback?.Invoke(obj);
    }

    public virtual void Save(IReadOnlyCollection<T> objs)
    {
        if (objs.Count == 0)
        {
            return;
        }

        foreach (var obj in objs)
        {
            BeginSaveCallback?.Invoke(obj);
        }

        Lock(() =>
        {
            using var context = GetDbContext();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                foreach (var obj in objs)
                {
                    var entry = context.Entry(obj);
                    if (entry.IsKeySet)
                    {
                        context.Update(obj);
                    }
                    else
                    {
                        context.Add(obj);
                    }
                }
                context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });

        using (var session = _databaseFactory.OpenSessionWrapper(useEntityFramework: true))
        {
            foreach (var obj in objs)
            {
                SaveWithOpenTransactionCallback?.Invoke(session, obj);
            }
        }

        WriteLock(
            () =>
            {
                foreach (var obj in objs)
                {
                    UpdateCacheUnsafe(obj);
                }
            }
        );

        foreach (var obj in objs)
        {
            EndSaveCallback?.Invoke(obj);
        }
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public virtual void SaveWithOpenTransaction(ISessionWrapper session, T obj)
    {
        Lock(() =>
        {
            if (Equals(SelectKey(obj), default(S)))
            {
                session.Insert(obj);
            }
            else
            {
                session.Update(obj);
            }
        });

        SaveWithOpenTransactionCallback?.Invoke(session, obj);
        WriteLock(() => UpdateCacheUnsafe(obj));
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public virtual void SaveWithOpenTransaction(ISession session, T obj)
    {
        Lock(() => session.SaveOrUpdate(obj));

        SaveWithOpenTransactionCallback?.Invoke(session.Wrap(), obj);

        WriteLock(() => UpdateCacheUnsafe(obj));
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public void SaveWithOpenTransaction(ISession session, List<T> objs)
    {
        if (objs.Count == 0)
        {
            return;
        }

        Lock(() =>
        {
            foreach (var obj in objs)
            {
                session.SaveOrUpdate(obj);
            }
        });

        foreach (var obj in objs)
        {
            SaveWithOpenTransactionCallback?.Invoke(session.Wrap(), obj);
        }

        foreach (var obj in objs)
        {
            WriteLock(() => UpdateCacheUnsafe(obj));
        }
    }

    public async Task SaveWithOpenTransactionAsync(ISession session, IReadOnlyList<T> objs)
    {
        if (objs.Count == 0) return;

        await Lock(async () =>
        {
            foreach (var obj in objs)
            {
                await session.SaveOrUpdateAsync(obj);
            }
        });

        foreach (var obj in objs)
        {
            SaveWithOpenTransactionCallback?.Invoke(session.Wrap(), obj);
        }

        foreach (var obj in objs)
        {
            WriteLock(() => UpdateCacheUnsafe(obj));
        }
    }

    protected T5 ReadLock<T5>(Func<T5> action)
    {
        _lock.EnterReadLock();
        try
        {
            return action.Invoke();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    protected void WriteLock(Action action)
    {
        _lock.EnterWriteLock();
        try
        {
            action.Invoke();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    protected T5 WriteLock<T5>(Func<T5> action)
    {
        _lock.EnterWriteLock();
        try
        {
            return action.Invoke();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    #region Unsafe

    public virtual void ClearCacheUnsafe()
    {
        Cache.Clear();
    }

    protected virtual T GetByIDUnsafe(S id)
    {
        return Cache.Get(id);
    }

    protected virtual IReadOnlyList<T> GetAllUnsafe()
    {
        return Cache.Values.ToList();
    }

    protected virtual IReadOnlyList<T> GetAllUnsafe(int maxLimit)
    {
        return Cache.Values.Take(maxLimit).ToList();
    }

    protected virtual void UpdateCacheUnsafe(T cr)
    {
        Cache.Update(cr);
    }

    protected virtual void DeleteFromCacheUnsafe(T cr)
    {
        Cache.Remove(cr);
    }

    private void DeleteFromDatabaseUnsafe(T cr)
    {
        using var context = GetDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
        {
            using var session = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            DeleteWithOpenTransactionCallback?.Invoke(session, cr);
            context.Remove(cr);
            context.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private void DeleteFromDatabaseUnsafe(IReadOnlyCollection<T> objs)
    {
        using var context = GetDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
        {
            using var session = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            foreach (var cr in objs)
            {
                DeleteWithOpenTransactionCallback?.Invoke(session, cr);
                context.Remove(cr);
            }
            context.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    #endregion

    #region abstract

    public virtual void PopulateIndexes() { }

    public virtual void RegenerateDb() { }

    public virtual void PostProcess() { }

    protected abstract S SelectKey(T entity);

    #endregion
}
