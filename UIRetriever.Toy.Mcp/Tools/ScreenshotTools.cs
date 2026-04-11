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

    [McpServerTool(Name = "grided_screenshot")]
    [Description("Take a screenshot of the entire primary screen and overlay a 16×9 grid of 1-pixel black lines. " +
                 "Returns the result as a PNG image. Grid cells can be referenced by column (0-15) and row (0-8) " +
                 "for approximate coordinate estimation.")]
    public static IList<ContentBlock> TakeGridedScreenshot(
        [Description("Optional absolute path to a known-good PNG file to use as the base image instead of capturing the screen.")] string? pngFilePath = null)
    {
        byte[] baseBytes;
        if (!string.IsNullOrWhiteSpace(pngFilePath) && File.Exists(pngFilePath))
        {
            baseBytes = File.ReadAllBytes(pngFilePath);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(pngFilePath))
                Console.WriteLine($"Requested pngFilePath not found on server: {pngFilePath}. Falling back to live capture.");
            baseBytes = CaptureScreenshotBytes();
        }

        byte[] gridBytes = OverlayGrid(baseBytes, columns: 16, rows: 9);
        return [ImageContentBlock.FromBytes(gridBytes, "image/png")];
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

    private static byte[] OverlayGrid(byte[] pngBytes, int columns, int rows)
    {
        using var inputStream = new MemoryStream(pngBytes);
        using var bitmap = new Bitmap(inputStream);

        using (var g = Graphics.FromImage(bitmap))
        using (var pen = new Pen(Color.Black, 1))
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            // Vertical lines (columns - 1 internal lines)
            for (int col = 1; col < columns; col++)
            {
                int x = (int)Math.Round((double)col * w / columns);
                g.DrawLine(pen, x, 0, x, h - 1);
            }

            // Horizontal lines (rows - 1 internal lines)
            for (int row = 1; row < rows; row++)
            {
                int y = (int)Math.Round((double)row * h / rows);
                g.DrawLine(pen, 0, y, w - 1, y);
            }
        }

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
