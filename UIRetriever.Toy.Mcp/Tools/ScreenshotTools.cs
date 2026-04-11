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
    private static readonly string ScreenshotFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "UIRetriever", "Screenshots");

    [McpServerTool(Name = "screenshot")]
    [Description("Take a screenshot of the entire primary screen at native resolution and return it as a PNG image. " +
                 "The image is NOT resized so that pixel coordinates match actual screen coordinates for UI automation.")]
    public static IList<ContentBlock> TakeScreenshot(
        [Description("Optional absolute path to a known-good PNG file to return instead of capturing the screen.")] string? pngFilePath = null,
        [Description("When true, save the PNG to the AppData screenshots folder and include the file path in the response.")] bool saveToFile = false)
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

        return BuildResponse(bytes, saveToFile, "screenshot");
    }

    [McpServerTool(Name = "grided_screenshot")]
    [Description("Take a screenshot of the entire primary screen and overlay a 16×9 grid of 2-pixel-thick lines " +
                 "whose color is the negative (inverted) of the underlying background, ensuring visibility on any content. " +
                 "Returns the result as a PNG image. Grid cells can be referenced by column (0-15) and row (0-8) " +
                 "for approximate coordinate estimation.")]
    public static IList<ContentBlock> TakeGridedScreenshot(
        [Description("Optional absolute path to a known-good PNG file to use as the base image instead of capturing the screen.")] string? pngFilePath = null,
        [Description("When true, save the PNG to the AppData screenshots folder and include the file path in the response.")] bool saveToFile = false)
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
        return BuildResponse(gridBytes, saveToFile, "grided_screenshot");
    }

    private static IList<ContentBlock> BuildResponse(byte[] pngBytes, bool saveToFile, string prefix)
    {
        var blocks = new List<ContentBlock> { ImageContentBlock.FromBytes(pngBytes, "image/png") };

        if (saveToFile)
        {
            string savedPath = SaveToAppData(pngBytes, prefix);
            blocks.Add(new TextContentBlock { Text = $"Saved to: {savedPath}" });
        }

        return blocks;
    }

    private static string SaveToAppData(byte[] pngBytes, string prefix)
    {
        Directory.CreateDirectory(ScreenshotFolder);
        string fileName = $"{prefix}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png";
        string fullPath = Path.Combine(ScreenshotFolder, fileName);
        File.WriteAllBytes(fullPath, pngBytes);
        return fullPath;
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
        const int thickness = 2;

        using var inputStream = new MemoryStream(pngBytes);
        using var bitmap = new Bitmap(inputStream);

        int w = bitmap.Width;
        int h = bitmap.Height;

        // Collect grid-line pixel coordinates
        var verticalXs = new HashSet<int>();
        for (int col = 1; col < columns; col++)
        {
            int cx = (int)Math.Round((double)col * w / columns);
            for (int t = 0; t < thickness; t++)
            {
                int x = cx - thickness / 2 + t;
                if (x >= 0 && x < w) verticalXs.Add(x);
            }
        }

        var horizontalYs = new HashSet<int>();
        for (int row = 1; row < rows; row++)
        {
            int cy = (int)Math.Round((double)row * h / rows);
            for (int t = 0; t < thickness; t++)
            {
                int y = cy - thickness / 2 + t;
                if (y >= 0 && y < h) horizontalYs.Add(y);
            }
        }

        // Invert pixels on grid lines using direct pixel access
        var rect = new Rectangle(0, 0, w, h);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        try
        {
            unsafe
            {
                byte* scan0 = (byte*)data.Scan0;
                int stride = data.Stride;

                // Horizontal lines (full width)
                foreach (int y in horizontalYs)
                {
                    byte* rowPtr = scan0 + y * stride;
                    for (int x = 0; x < w; x++)
                        InvertPixel(rowPtr + x * 4);
                }

                // Vertical lines (skip already-inverted intersections)
                foreach (int x in verticalXs)
                {
                    for (int y = 0; y < h; y++)
                    {
                        if (horizontalYs.Contains(y)) continue;
                        InvertPixel(scan0 + y * stride + x * 4);
                    }
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    private static unsafe void InvertPixel(byte* pixel)
    {
        pixel[0] = (byte)(255 - pixel[0]); // B
        pixel[1] = (byte)(255 - pixel[1]); // G
        pixel[2] = (byte)(255 - pixel[2]); // R
        // pixel[3] (A) is left unchanged
    }
}
