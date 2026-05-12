using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MessagePack;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable enable

namespace Shoko.Server.Data.Converters;

/// <summary>
/// Static helper methods for MessagePack serialization used by <see cref="MessagePackConverter{T}"/>.
///
/// These methods use the non-generic MessagePack API which is always available at design-time
/// (unlike source-generated generic methods such as Serialize&lt;T&gt;).
/// </summary>
public static class MessagePackHelpers
{
    public static byte[]? Serialize<T>(T value)
        where T : class
    {
        if (value == null) return null;
        return MessagePackSerializer.Serialize(value);
    }

    public static T? Deserialize<T>(byte[]? bytes)
        where T : class
    {
        if (bytes == null) return null;
        return MessagePackSerializer.Deserialize<T>(bytes);
    }

    public static byte[]? SerializeTypeless(object value)
    {
        if (value == null) return null;
        return MessagePackSerializer.Typeless.Serialize(value);
    }

    public static object? DeserializeTypeless(byte[]? bytes)
    {
        if (bytes == null) return null;
        return MessagePackSerializer.Typeless.Deserialize(bytes);
    }
}

/// <summary>
/// EF Core ValueConverter&lt;T, byte[]&gt; that serializes/deserializes using MessagePack.
/// Preserves exact MessagePack payload format from NHibernate MessagePackConverter&lt;T&gt;.
/// </summary>
/// <typeparam name="T">The entity type to serialize. Must be a class.</typeparam>
public class MessagePackConverter<T> : ValueConverter<T, byte[]>
    where T : class
{
    public MessagePackConverter()
        : base(
            v => SerializeWrap(v),
            b => DeserializeWrap(b))
    {
    }

    private MessagePackConverter(
        Expression<Func<T, byte[]>> convertToProvider,
        Expression<Func<byte[], T>> convertFromProvider)
        : base(convertToProvider, convertFromProvider)
    {
    }

    private static byte[]? SerializeWrap(T v)
    {
        try { return MessagePackHelpers.Serialize(v); }
        catch { return null; }
    }

    private static T? DeserializeWrap(byte[]? b)
    {
        try { return MessagePackHelpers.Deserialize<T>(b); }
        catch { return null; }
    }

    /// <summary>
    /// Creates a converter that uses typeless serialization for <c>object</c> types.
    /// </summary>
    public static MessagePackConverter<object> CreateTypeless()
    {
        return new MessagePackConverter<object>(
            v => SerializeTypelessWrap(v),
            b => DeserializeTypelessWrap(b));
    }

    private static byte[]? SerializeTypelessWrap(object v)
    {
        try { return MessagePackHelpers.SerializeTypeless(v); }
        catch { return null; }
    }

    private static object? DeserializeTypelessWrap(byte[]? b)
    {
        try { return MessagePackHelpers.DeserializeTypeless(b); }
        catch { return null; }
    }
}
