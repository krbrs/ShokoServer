using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shoko.Server.Databases.NHibernate;
using Shoko.Server.Filters;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;FilterExpression&lt;bool&gt;, string&gt; that serializes/deserializes
/// FilterExpression&lt;bool&gt; using Newtonsoft.Json with TypeNameHandling.Objects.
/// Preserves exact JSON format from NHibernate FilterExpressionConverter.
/// </summary>
public class FilterExpressionConverter : ValueConverter<FilterExpression<bool>, string>
{
    private static readonly JsonSerializerSettings _serializeSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Objects,
        SerializationBinder = new SimpleNameSerializationBinder(typeof(FilterExpression<bool>)),
    };

    private static readonly JsonSerializerSettings _deserializeSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Objects,
        SerializationBinder = new SimpleNameSerializationBinder(typeof(FilterExpression<bool>)),
        Error = (_, args) =>
        {
            args.ErrorContext.Handled = true;
        },
    };

    public FilterExpressionConverter()
        : base(v => ToJson(v), s => FromJson(s))
    {
    }

    private static string ToJson(FilterExpression<bool> value)
    {
        if (value == null) return null;
        return JsonConvert.SerializeObject(value, _serializeSettings);
    }

    private static FilterExpression<bool> FromJson(string json)
    {
        if (json == null) return null;
        return (FilterExpression<bool>)JsonConvert.DeserializeObject(json, typeof(FilterExpression<bool>), _deserializeSettings);
    }
}
