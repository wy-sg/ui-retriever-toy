namespace UIRetriever.Contracts.Barks;

/// <summary>
/// Request to invoke (activate/press) a marked UI element.
/// </summary>
public sealed class InvokeRequest
{
    /// <summary>Name of the mark identifying the target element.</summary>
    public string MarkName { get; set; } = string.Empty;
}
