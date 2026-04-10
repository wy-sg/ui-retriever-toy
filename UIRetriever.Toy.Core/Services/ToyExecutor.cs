using UIRetriever.Contracts.Barks;
using UIRetriever.Contracts.Results;

namespace UIRetriever.Toy.Core.Services;

/// <summary>
/// Placeholder toy executor. Returns not-implemented results.
/// Replace with real implementation during migration.
/// </summary>
public sealed class ToyExecutor : IToyExecutor
{
    public Task<BarkResult> InvokeAsync(InvokeRequest request, CancellationToken ct = default)
        => Task.FromResult(BarkResult.Fail("Not implemented", "NotImplemented"));

    public Task<BarkResult> SetTextAsync(SetTextRequest request, CancellationToken ct = default)
        => Task.FromResult(BarkResult.Fail("Not implemented", "NotImplemented"));

    public Task<BarkResult> ClickElementAsync(ClickElementRequest request, CancellationToken ct = default)
        => Task.FromResult(BarkResult.Fail("Not implemented", "NotImplemented"));

    public Task<BarkResult> RightClickElementAsync(RightClickElementRequest request, CancellationToken ct = default)
        => Task.FromResult(BarkResult.Fail("Not implemented", "NotImplemented"));
}
