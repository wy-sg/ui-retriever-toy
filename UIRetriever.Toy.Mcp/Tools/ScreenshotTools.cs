using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace UIRetriever.Toy.Mcp.Tools;

[McpServerToolType]
public sealed class ScreenshotTools
{
    [McpServerTool(Name = "screenshot")]
    [Description("Take a screenshot of the entire primary screen at native resolution and return it as a PNG image. " +
                 "The image is NOT resized so that pixel coordinates match actual screen coordinates for UI automation.")]
    public static IList<ContentBlock> TakeScreenshot(
        [Description("Optional absolute path to a known-good PNG file to return instead of capturing the screen.")] string? pngFilePath = null)
    {
        byte[] bytes;
        if (!string.IsNullOrWhiteSpace(pngFilePath))
        {
            if (File.Exists(pngFilePath))
            {
                bytes = File.ReadAllBytes(pngFilePath);
            }
            else
            {
                Console.WriteLine($"Requested pngFilePath not found on server: {pngFilePath}. Falling back to live capture.");
                bytes = CaptureScreenshotBytes();
            }
        }
        else
        {
            bytes = CaptureScreenshotBytes();
        }

        return [ImageContentBlock.FromBytes(bytes, "image/png")];
    }

    private static byte[] CaptureScreenshotBytes()
    {
        var screen = Screen.PrimaryScreen!;
        var bounds = screen.Bounds;

        using var capture = new Bitmap(bounds.Width, bounds.Height);
        using (var g = Graphics.FromImage(capture))
            g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);

        using var ms = new MemoryStream();
        capture.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
