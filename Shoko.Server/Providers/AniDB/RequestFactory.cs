﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shoko.Server.Providers.AniDB.Interfaces;

namespace Shoko.Server.Providers.AniDB
{
    public class RequestFactory : IRequestFactory
    {
        private readonly IServiceProvider _provider;
        private static MethodInfo _cachedBaseMethod;
        private static readonly ConcurrentDictionary<string, MethodInfo> CachedGenericMethods = new();

        public RequestFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T Create<T>(Action<T> ctor = null) where T : class, IRequest
        {
            var baseType = typeof(T);
            while (baseType?.BaseType != null)
            {
                baseType = baseType.BaseType;
                if (baseType.GetInterfaces().Any(a => a == typeof(IRequest))) break;
            }

            if (baseType is not { IsGenericType: true }) throw new ArgumentException($"Type parameter {baseType} must be a generic IRequest type");
            var genericType = baseType.GetGenericArguments().FirstOrDefault();
            if (genericType == null) throw new ArgumentException($"Type parameter {baseType} must be a generic IRequest type");

            if (_cachedBaseMethod == null)
            {
                var methodInfo = GetType().GetMethods().FirstOrDefault(a => a.Name.Equals(nameof(Create)) && a.GetGenericArguments().Length == 2);
                if (methodInfo == null) throw new MissingMethodException(nameof(RequestFactory), nameof(Create));
                _cachedBaseMethod = methodInfo;
            }

            MethodInfo genericMethod;
            object result;
            var key = baseType.FullName + "," + genericType.FullName;
            if (!CachedGenericMethods.ContainsKey(key))
            {
                genericMethod = _cachedBaseMethod.MakeGenericMethod(typeof(T), genericType);
                // we don't care if there was a conflict. This is a cache that will have the same values
                CachedGenericMethods.TryAdd(key, genericMethod);
                result = genericMethod.Invoke(this, new object[] { ctor });
                return result as T;
            }

            genericMethod = CachedGenericMethods[key];
            result = genericMethod.Invoke(this, new object[] { ctor });
            return result as T;
        }

        public T Create<T,T1>(Action<T> ctor = null) where T : IRequest<IResponse<T1>, T1> where T1 : class
        {
            var obj = ActivatorUtilities.CreateInstance<T>(_provider);
            ctor?.Invoke(obj);
            return obj;
        }
    }
}
