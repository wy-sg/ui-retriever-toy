namespace UIRetriever.Contracts.Barks;

/// <summary>
/// Request to set focus on a marked UI element.
/// </summary>
public sealed class SetFocusRequest
{
    /// <summary>Name of the mark identifying the target element.</summary>
    public string MarkName { get; set; } = string.Empty;
}
