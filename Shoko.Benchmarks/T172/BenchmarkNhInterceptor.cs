using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Type;

namespace Benchmarks.T172;

internal sealed class BenchmarkNhInterceptor : EmptyInterceptor
{
    private readonly IServiceProvider _provider;
    private ISession _session;

    public BenchmarkNhInterceptor(IServiceProvider provider)
    {
        _provider = provider;
    }

    public override void SetSession(ISession session)
    {
        _session = session;
    }

    public override object Instantiate(string clazz, object id)
    {
        var type = Type.GetType(clazz);
        if (type == null) return null;

        var constructors = type.GetConstructors();
        var hasParameters = constructors.Any(c => c.GetParameters().Any());
        if (!hasParameters) return null;

        try
        {
            var instance = Activator.CreateInstance(type);
            var md = _session?.SessionFactory?.GetClassMetadata(type);
            md?.SetIdentifier(instance, id);
            return instance;
        }
        catch
        {
            return null;
        }
    }
}