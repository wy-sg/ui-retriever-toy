namespace UIRetriever.Schema;

/// <summary>
/// How a mark entry resolves to a UI element.
/// </summary>
public enum MarkEntryMethod
{
    /// <summary>Walk the UI tree using the node chain.</summary>
    Chain = 0,

    /// <summary>Direct AutomationId lookup (no chain walk).</summary>
    AutomationIdOnly = 1
}

/// <summary>
/// A single mark entry in a marks file.
/// Represents a saved path to a UI element in a target application.
/// </summary>
public sealed class MarkEntry
{
    /// <summary>Unique name identifying this mark.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Process name of the target application.</summary>
    public string ProcessName { get; set; } = string.Empty;

    /// <summary>Resolution method.</summary>
    public MarkEntryMethod Method { get; set; } = MarkEntryMethod.Chain;

    /// <summary>AutomationId for direct lookup (when Method is AutomationIdOnly).</summary>
    public string? DirectAutomationId { get; set; }

    /// <summary>Element chain from the process window root to the target element.</summary>
    public List<MarkNodeEntry> Chain { get; set; } = [];
}
