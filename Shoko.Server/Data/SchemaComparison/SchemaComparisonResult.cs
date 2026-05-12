using System.Collections.Generic;

#nullable enable

namespace Shoko.Server.Data.SchemaComparison;

/// <summary>
/// Result of a schema comparison between an EF Core model and an actual database.
/// </summary>
public class SchemaComparisonResult
{
    public string ProviderName { get; set; } = string.Empty;
    public string ConnectionInfo { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public List<SchemaError> Errors { get; set; } = [];
    public List<SchemaWarning> Warnings { get; set; } = [];
    public TableComparisonSummary TableSummary { get; set; } = new();
    public Dictionary<string, ColumnComparisonSummary> ColumnSummaries { get; set; } = new();
}

/// <summary>
/// Summary of table comparison results.
/// </summary>
public class TableComparisonSummary
{
    public int ExpectedTables { get; set; }
    public int ActualTables { get; set; }
    public int MatchingTables { get; set; }
    public int MissingTables { get; set; }
    public int ExtraTables { get; set; }
    public List<string> MissingTableNames { get; set; } = [];
    public List<string> ExtraTableNames { get; set; } = [];
}

/// <summary>
/// Summary of column comparison for a single table.
/// </summary>
public class ColumnComparisonSummary
{
    public string TableName { get; set; } = string.Empty;
    public int ExpectedColumns { get; set; }
    public int ActualColumns { get; set; }
    public int MatchingColumns { get; set; }
    public int MissingColumns { get; set; }
    public int ExtraColumns { get; set; }
    public List<string> MissingColumnNames { get; set; } = [];
    public List<string> ExtraColumnNames { get; set; } = [];
}

/// <summary>
/// Represents a schema discrepancy error.
/// </summary>
public class SchemaError
{
    public string Category { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ExpectedValue { get; set; } = string.Empty;
    public string ActualValue { get; set; } = string.Empty;
}

/// <summary>
/// Represents a schema discrepancy warning.
/// </summary>
public class SchemaWarning
{
    public string Category { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
