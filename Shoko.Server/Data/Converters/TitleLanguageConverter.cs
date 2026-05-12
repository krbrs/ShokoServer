using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Metadata.Enums;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;TitleLanguage, string&gt; that serializes TitleLanguage to/from
/// IETF language code strings using the same format as NHibernate TitleLanguageConverter.
/// </summary>
public class TitleLanguageConverter : ValueConverter<TitleLanguage, string>
{
    public TitleLanguageConverter()
        : base(v => ToString(v), s => FromString(s))
    {
    }

    private static string ToString(TitleLanguage value)
    {
        return value.GetString();
    }

    private static TitleLanguage FromString(string value)
    {
        if (value == null) return TitleLanguage.Unknown;
        return value.GetTitleLanguage();
    }
}
