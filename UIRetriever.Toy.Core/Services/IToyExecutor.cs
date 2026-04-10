using UIRetriever.Contracts.Barks;
using UIRetriever.Contracts.Results;

namespace UIRetriever.Toy.Core.Services;

/// <summary>
/// Toy-safe executor interface for simple bark operations.
/// Supports only basic operations against simple marks.
/// Does NOT support errands, tricks, or procedures.
/// </summary>
public interface IToyExecutor
{
    Task<BarkResult> InvokeAsync(InvokeRequest request, CancellationToken ct = default);
    Task<BarkResult> SetTextAsync(SetTextRequest request, CancellationToken ct = default);
    Task<BarkResult> ClickElementAsync(ClickElementRequest request, CancellationToken ct = default);
    Task<BarkResult> RightClickElementAsync(RightClickElementRequest request, CancellationToken ct = default);
}
