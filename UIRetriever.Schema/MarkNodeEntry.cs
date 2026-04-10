namespace UIRetriever.Schema;

/// <summary>
/// Search scope for a mark node.
/// </summary>
public enum MarkNodeSearchScope
{
    Children,
    Descendants
}

/// <summary>
/// A single node in a mark entry's element chain.
/// Each node describes one level in the path from the process window
/// root to the target UI element.
/// </summary>
public sealed class MarkNodeEntry
{
    public string? Name { get; set; }
    public string? ClassName { get; set; }
    public int? ControlTypeId { get; set; }
    public string? AutomationId { get; set; }
    public int IndexAmongMatches { get; set; }
    public bool Include { get; set; } = true;
    public bool UseName { get; set; }
    public bool UseClassName { get; set; } = true;
    public bool UseControlTypeId { get; set; } = true;
    public bool UseAutomationId { get; set; } = true;
    public bool UseIndex { get; set; } = true;
    public MarkNodeSearchScope Scope { get; set; } = MarkNodeSearchScope.Children;
}
