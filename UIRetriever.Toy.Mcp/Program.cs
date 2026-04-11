// UIRetriever.Toy.Mcp — Unified MCP Server (Mark Authoring + Bark Execution)
//
// Mark tools:
//   mark_element    — pick + tune + save a UI element as a named mark
//   list_marks      — list all saved marks
//   validate_mark   — resolve a mark and report success/failure
//   delete_mark     — delete a saved mark
//
// Bark tools:
//   invoke              — invoke/activate a marked element
//   set_text            — set text on a marked element
//   click_element       — click a marked element
//   right_click_element — right-click a marked element
//
// Utility tools:
//   screenshot      — capture the entire screen and return as PNG image
//
// Transport: MCP Streamable HTTP on port 8000

using ModelContextProtocol.AspNetCore;
using UIRetriever.Toy.Mcp.Tools;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:8000");

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<MarkTools>()
    .WithTools<BarkTools>()
    .WithTools<ScreenshotTools>();

var app = builder.Build();

app.MapMcp();

// Debug helper: POST JSON to /debug/screenshot to force a screenshot (or return an existing PNG).
// Body example: { "pngFilePath": "C:\\temp\\known_good.png", "maxWidth": 1024 }
app.MapPost("/debug/screenshot", async (HttpRequest req) =>
{
    using var doc = await JsonDocument.ParseAsync(req.Body);
    var root = doc.RootElement;

    string? pngFilePath = null;
    if (root.TryGetProperty("pngFilePath", out var p) && p.ValueKind == JsonValueKind.String)
        pngFilePath = p.GetString();

    int maxWidth = 1280;
    if (root.TryGetProperty("maxWidth", out var w) && w.TryGetInt32(out var wi))
        maxWidth = wi;

    // Call the tool which writes the debug copy to disk and returns the content block.
    // Optionally request the server to additionally write the returned bytes to a custom path
    // so curl callers can inspect the exact file (saveFilePath).
    string? saveFilePath = null;
    if (root.TryGetProperty("saveFilePath", out var sp) && sp.ValueKind == JsonValueKind.String)
        saveFilePath = sp.GetString();

    _ = ScreenshotTools.TakeScreenshot(maxWidth, 70, pngFilePath, saveFilePath);

    var debugPath = ScreenshotTools.Step1ImagePath;
    if (!System.IO.File.Exists(debugPath))
        return Results.NotFound("debug image not found on server");

    var bytes = await System.IO.File.ReadAllBytesAsync(debugPath);
    ScreenshotTools.SaveArtifact(ScreenshotTools.Step2ImagePath, bytes, "step2-http");

    var step1Hash = ScreenshotTools.ComputeSha256(bytes);
    var step2Hash = step1Hash;
    req.HttpContext.Response.Headers.Append("X-Step1-Sha256", step1Hash);
    req.HttpContext.Response.Headers.Append("X-Step2-Sha256", step2Hash);

    return Results.File(bytes, "image/png", "screenshot.png");
});

app.MapPost("/debug/screenshot/verify-client", async (HttpRequest req) =>
{
    using var ms = new MemoryStream();
    await req.Body.CopyToAsync(ms);
    var clientBytes = ms.ToArray();

    if (clientBytes.Length == 0)
        return Results.BadRequest(new { error = "empty request body" });

    var clientHash = ScreenshotTools.ComputeSha256(clientBytes);

    if (!System.IO.File.Exists(ScreenshotTools.Step1ImagePath))
        return Results.NotFound(new { error = "step1 image not found", path = ScreenshotTools.Step1ImagePath });

    var step1Bytes = await System.IO.File.ReadAllBytesAsync(ScreenshotTools.Step1ImagePath);
    var step1Hash = ScreenshotTools.ComputeSha256(step1Bytes);
    var matches = string.Equals(clientHash, step1Hash, StringComparison.OrdinalIgnoreCase);

    if (matches)
        ScreenshotTools.SaveArtifact(ScreenshotTools.Step3ImagePath, clientBytes, "step3-client");

    return Results.Json(new
    {
        step1Hash,
        clientHash,
        matches,
        savedPath = matches ? ScreenshotTools.Step3ImagePath : null
    });
});

app.Run();
