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

app.Run();
