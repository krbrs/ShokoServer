using System;
using System.Linq.Expressions;
using System.Reflection;
using MessagePack;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;object, byte[]&gt; that serializes/deserializes using MessagePack.Typeless.
/// Preserves exact MessagePack.Typeless payload format from NHibernate TypelessMessagePackConverter.
/// </summary>
public class TypelessMessagePackConverter : ValueConverter<object, byte[]>
{
    public TypelessMessagePackConverter()
        : base(CreateSerializeExpression(), CreateDeserializeExpression())
    {
    }

    private static MethodInfo TypelessPropertyGetter => typeof(MessagePackSerializer).GetProperty(nameof(MessagePackSerializer.Typeless))!.GetGetMethod()!;

    private static Expression<Func<object, byte[]>> CreateSerializeExpression()
    {
        var param = Expression.Parameter(typeof(object), "v");
        var resultLabel = Expression.Label(typeof(byte[]));
        var nullCheck = Expression.IfThen(Expression.Equal(param, Expression.Constant(null)),
            Expression.Return(resultLabel, Expression.Constant(null, typeof(byte[]))));

        var typelessObj = Expression.Call(null, TypelessPropertyGetter);
        var serializeBody = Expression.Block(
            Expression.Return(resultLabel,
                Expression.Call(typelessObj, "Serialize", Type.EmptyTypes, param)));

        var tryCatch = Expression.TryCatch(
            Expression.Block(nullCheck, serializeBody),
            Expression.Catch(typeof(Exception),
                Expression.Return(resultLabel, Expression.Constant(null, typeof(byte[])))));

        return Expression.Lambda<Func<object, byte[]>>(
            Expression.Block(new[] { param },
                Expression.Label(resultLabel, Expression.Constant(null, typeof(byte[]))),
                tryCatch),
            param);
    }

    private static Expression<Func<byte[], object>> CreateDeserializeExpression()
    {
        var param = Expression.Parameter(typeof(byte[]), "b");
        var resultLabel = Expression.Label(typeof(object));
        var nullCheck = Expression.IfThen(Expression.Equal(param, Expression.Constant(null)),
            Expression.Return(resultLabel, Expression.Constant(null, typeof(object))));

        var typelessObj = Expression.Call(null, TypelessPropertyGetter);
        var deserializeBody = Expression.Block(
            Expression.Return(resultLabel,
                Expression.Call(typelessObj, "Deserialize", Type.EmptyTypes, param)));

        var tryCatch = Expression.TryCatch(
            Expression.Block(nullCheck, deserializeBody),
            Expression.Catch(typeof(Exception),
                Expression.Return(resultLabel, Expression.Constant(null, typeof(object)))));

        return Expression.Lambda<Func<byte[], object>>(
            Expression.Block(new[] { param },
                Expression.Label(resultLabel, Expression.Constant(null, typeof(object))),
                tryCatch),
            param);
    }
}
