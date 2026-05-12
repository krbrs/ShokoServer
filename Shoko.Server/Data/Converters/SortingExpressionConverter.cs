using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Shoko.Server.Databases.NHibernate;
using Shoko.Server.Filters;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;SortingExpression, string&gt; that serializes/deserializes
/// SortingExpression using Newtonsoft.Json with TypeNameHandling.Objects.
/// Preserves exact JSON format from NHibernate FilterExpressionConverter.
/// </summary>
public class SortingExpressionConverter : ValueConverter<SortingExpression, string>
{
    private static readonly JsonSerializerSettings _serializeSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Objects,
        SerializationBinder = new SimpleNameSerializationBinder(typeof(SortingExpression)),
    };

    private static readonly JsonSerializerSettings _deserializeSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Objects,
        SerializationBinder = new SimpleNameSerializationBinder(typeof(SortingExpression)),
        Error = (_, args) =>
        {
            args.ErrorContext.Handled = true;
        },
    };

    public SortingExpressionConverter()
        : base(v => ToJson(v), s => FromJson(s))
    {
    }

    private static string ToJson(SortingExpression value)
    {
        if (value == null) return null;
        return JsonConvert.SerializeObject(value, _serializeSettings);
    }

    private static SortingExpression FromJson(string json)
    {
        if (json == null) return null;
        return (SortingExpression)JsonConvert.DeserializeObject(json, typeof(SortingExpression), _deserializeSettings);
    }
}
