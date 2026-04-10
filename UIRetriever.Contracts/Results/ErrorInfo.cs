namespace UIRetriever.Contracts.Results;

/// <summary>
/// Error details for a failed bark operation.
/// </summary>
public sealed class ErrorInfo
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
