using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace UIRetriever.Toy.Mcp.Tools;

[McpServerToolType]
public sealed class ScreenshotTools
{
    [McpServerTool(Name = "screenshot")]
    [Description("Take a screenshot of the entire primary screen and return it as a PNG image for LLM analysis.")]
    public static IList<AIContent> TakeScreenshot()
    {
        var screen = Screen.PrimaryScreen!;
        var bounds = screen.Bounds;

        using var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);

        return [new DataContent(ms.ToArray(), "image/png")];
    }
}
