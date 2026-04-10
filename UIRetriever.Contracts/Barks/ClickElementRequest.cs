namespace UIRetriever.Contracts.Barks;

/// <summary>
/// Request to click a marked UI element.
/// </summary>
public sealed class ClickElementRequest
{
    /// <summary>Name of the mark identifying the target element.</summary>
    public string MarkName { get; set; } = string.Empty;
}
