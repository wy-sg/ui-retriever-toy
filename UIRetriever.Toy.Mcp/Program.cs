// UIRetriever.Toy.Mcp — Public Lite Executor MCP Host
//
// Role: Executes simple barks against simple marks from the marks file.
// Transport: JSON-RPC 2.0 over stdio.
//
// Supported bark tools:
//   invoke              — invoke/activate a marked element
//   set_text            — set text on a marked element
//   click_element       — click a marked element
//   right_click_element — right-click a marked element
//
// This host does NOT support errands, tricks, or procedures.
// It is a simple, public, playground-level executor.

Console.Error.WriteLine("UIRetriever.Toy.Mcp starting...");
Console.Error.WriteLine("Toy executor MCP host ready. Waiting for JSON-RPC messages...");

// TODO: Wire up DI container
// TODO: Load marks from marks file
// TODO: Register bark tools (invoke, set_text, click_element, right_click_element)
// TODO: Start JSON-RPC transport loop

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException)
{
    // Clean shutdown
}

Console.Error.WriteLine("UIRetriever.Toy.Mcp stopped.");
