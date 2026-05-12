using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;Type, string&gt; that serializes Type to/from its full name string.
/// On deserialization, attempts Type.GetType first, then searches all loaded assemblies.
/// Preserves exact behavior from NHibernate TypeStringConverter.
/// </summary>
public class TypeStringConverter : ValueConverter<Type, string>
{
    public TypeStringConverter()
        : base(v => ToString(v), s => FromString(s))
    {
    }

    private static string ToString(Type value)
    {
        if (value == null) return null;
        return value.ToString();
    }

    private static Type FromString(string value)
    {
        if (value == null) return null;
        var type = Type.GetType(value);
        if (type != null) return type;
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name.Equals(value) || t.FullName.Equals(value));
    }
}
