using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Shoko.Server.Databases;
using Shoko.Server.Repositories.NHibernate;

// ReSharper disable InconsistentNaming

namespace Shoko.Server.Repositories;

public class BaseDirectRepository<T, S> : BaseRepository, IDirectRepository, IRepository<T, S> where T : class
{
    protected readonly DatabaseFactory _databaseFactory;

    public BaseDirectRepository(DatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public Action<T> BeginDeleteCallback { get; set; }
    public Action<ISessionWrapper, T> DeleteWithOpenTransactionCallback { get; set; }
    public Action<T> EndDeleteCallback { get; set; }
    public Action<T> BeginSaveCallback { get; set; }
    public Action<ISessionWrapper, T> SaveWithOpenTransactionCallback { get; set; }
    public Action<T> EndSaveCallback { get; set; }

    public virtual T GetByID(S id)
    {
        return Lock(() =>
        {
            using var wrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            return wrapper.Get<T>(id);
        });
    }

    public virtual T GetByID(ISession session, S id)
    {
        return Lock(() => session.Get<T>(id));
    }

    public virtual T GetByID(ISessionWrapper session, S id)
    {
        return Lock(() => session.Get<T>(id));
    }

    public virtual IReadOnlyList<T> GetAll()
    {
        return Lock(() =>
        {
            using var wrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            return wrapper.Query<T>().ToList();
        });
    }

    public virtual IReadOnlyList<T> GetAll(ISession session)
    {
        return Lock(() => session.CreateCriteria(typeof(T)).List<T>().ToList());
    }

    public virtual IReadOnlyList<T> GetAll(ISessionWrapper session)
    {
        return Lock(() => session.CreateCriteria(typeof(T)).List<T>().ToList());
    }


    public virtual void Delete(S id)
    {
        Delete(GetByID(id));
    }

    public virtual void Delete(T cr)
    {
        if (cr == null) return;

        Lock(() =>
        {
            BeginDeleteCallback?.Invoke(cr);
            using var wrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            var transaction = wrapper.BeginTransaction();
            DeleteWithOpenTransactionCallback?.Invoke(wrapper, cr);
            wrapper.Delete(cr);
            transaction.Commit();
            EndDeleteCallback?.Invoke(cr);
        });
    }

    public void Delete(IReadOnlyCollection<T> objs)
    {
        if (objs.Count == 0) return;

        Lock(() =>
        {
            foreach (var obj in objs)
            {
                BeginDeleteCallback?.Invoke(obj);
            }

            using var wrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            var transaction = wrapper.BeginTransaction();
            foreach (var cr in objs)
            {
                DeleteWithOpenTransactionCallback?.Invoke(wrapper, cr);
                wrapper.Delete(cr);
            }

            transaction.Commit();

            foreach (var obj in objs)
            {
                EndDeleteCallback?.Invoke(obj);
            }
        });
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public virtual void DeleteWithOpenTransaction(ISession session, S id)
    {
        DeleteWithOpenTransaction(session, GetByID(id));
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public virtual void DeleteWithOpenTransaction(ISession session, T cr)
    {
        if (cr == null) return;

        Lock(() =>
        {
            DeleteWithOpenTransactionCallback?.Invoke(session.Wrap(), cr);
            session.Delete(cr);
        });
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public void DeleteWithOpenTransaction(ISession session, List<T> objs)
    {
        if (objs.Count == 0) return;

        Lock(() =>
        {
            foreach (var cr in objs)
            {
                DeleteWithOpenTransactionCallback?.Invoke(session.Wrap(), cr);
                session.Delete(cr);
            }
        });
    }

    public virtual void Save(T obj)
    {
        Lock(() =>
        {
            BeginSaveCallback?.Invoke(obj);
            using var wrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            var transaction = wrapper.BeginTransaction();
            wrapper.SaveOrUpdate(obj);
            SaveWithOpenTransactionCallback?.Invoke(wrapper, obj);
            transaction.Commit();
            EndSaveCallback?.Invoke(obj);
        });
    }

    public void Save(IReadOnlyCollection<T> objs)
    {
        if (objs.Count == 0) return;

        Lock(() =>
        {
            using var wrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            var transaction = wrapper.BeginTransaction();
            foreach (var obj in objs)
            {
                BeginSaveCallback?.Invoke(obj);
                wrapper.SaveOrUpdate(obj);
                SaveWithOpenTransactionCallback?.Invoke(wrapper, obj);
                EndSaveCallback?.Invoke(obj);
            }

            transaction.Commit();
        });
    }

    //This function do not run the BeginDeleteCallback and the EndDeleteCallback
    public void SaveWithOpenTransaction(ISession session, List<T> objs)
    {
        if (objs.Count == 0) return;

        Lock(() =>
        {
            foreach (var obj in objs)
            {
                session.SaveOrUpdate(obj);
                SaveWithOpenTransactionCallback?.Invoke(session.Wrap(), obj);
            }
        });
    }
}
