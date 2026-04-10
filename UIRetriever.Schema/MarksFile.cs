namespace UIRetriever.Schema;

/// <summary>
/// Root container for a marks file.
/// A marks file holds a collection of mark entries that identify UI elements
/// in one or more target applications.
/// </summary>
public sealed class MarksFile
{
    /// <summary>Schema version of this marks file.</summary>
    public string Version { get; set; } = SchemaVersion.Current;

    /// <summary>Optional description of what this marks file targets.</summary>
    public string? Description { get; set; }

    /// <summary>The mark entries in this file.</summary>
    public List<MarkEntry> Marks { get; set; } = [];
}
