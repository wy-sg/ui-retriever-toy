namespace UIRetriever.Contracts.Barks;

/// <summary>
/// Request to get the text/value of a marked UI element.
/// </summary>
public sealed class GetTextRequest
{
    /// <summary>Name of the mark identifying the target element.</summary>
    public string MarkName { get; set; } = string.Empty;
}
