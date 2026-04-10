namespace UIRetriever.Contracts.Barks;

/// <summary>
/// Request to right-click a marked UI element.
/// </summary>
public sealed class RightClickElementRequest
{
    /// <summary>Name of the mark identifying the target element.</summary>
    public string MarkName { get; set; } = string.Empty;
}
