namespace UIRetriever.Contracts.Barks;

/// <summary>
/// Request to set the text/value of a marked UI element.
/// </summary>
public sealed class SetTextRequest
{
    /// <summary>Name of the mark identifying the target element.</summary>
    public string MarkName { get; set; } = string.Empty;

    /// <summary>The text value to set.</summary>
    public string Value { get; set; } = string.Empty;
}
