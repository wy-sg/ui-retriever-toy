namespace UIRetriever.Contracts.Results;

/// <summary>
/// Result of a bark (atomic operation) execution.
/// </summary>
public sealed class BarkResult
{
    public bool Success { get; init; }
    public string? Value { get; init; }
    public ErrorInfo? Error { get; init; }

    public static BarkResult Ok(string? value = null)
        => new() { Success = true, Value = value };

    public static BarkResult Fail(string message, string? code = null)
        => new() { Success = false, Error = new ErrorInfo { Message = message, Code = code ?? "BarkFailed" } };
}
