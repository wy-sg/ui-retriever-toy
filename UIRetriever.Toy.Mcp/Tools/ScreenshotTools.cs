using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace UIRetriever.Toy.Mcp.Tools;

[McpServerToolType]
public sealed class ScreenshotTools
{
    private const string DebugOutputPath = @"C:\temp\mcp_exact_return.png";

    [McpServerTool(Name = "screenshot")]
    [Description("Take a screenshot of the entire primary screen and return it as a PNG image for LLM analysis. " +
                 "The image is scaled down to keep the payload small, and the exact returned bytes are also written to disk for diagnostics.")]
    public static IList<ContentBlock> TakeScreenshot(
        [Description("Maximum width in pixels (default 1280). Height is scaled proportionally.")] int maxWidth = 1280,
        [Description("Legacy JPEG quality parameter kept for compatibility. Ignored in PNG diagnostic mode.")] int quality = 70,
        [Description("Optional absolute path to a known-good PNG file to return instead of capturing the screen.")] string? pngFilePath = null)
    {
        _ = Math.Clamp(quality, 1, 100);
        maxWidth = Math.Clamp(maxWidth, 320, 3840);

        byte[] bytes;
        if (!string.IsNullOrWhiteSpace(pngFilePath))
        {
            bytes = File.ReadAllBytes(pngFilePath);
        }
        else
        {
            var screen = Screen.PrimaryScreen!;
            var bounds = screen.Bounds;

            using var capture = new Bitmap(bounds.Width, bounds.Height);
            using (var g = Graphics.FromImage(capture))
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);

            Bitmap output;
            if (capture.Width > maxWidth)
            {
                double scale = (double)maxWidth / capture.Width;
                int newW = maxWidth;
                int newH = (int)(capture.Height * scale);

                output = new Bitmap(newW, newH);
                using var g = Graphics.FromImage(output);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(capture, 0, 0, newW, newH);
            }
            else
            {
                output = capture;
            }

            try
            {
                using var ms = new MemoryStream();
                output.Save(ms, ImageFormat.Png);
                bytes = ms.ToArray();
            }
            finally
            {
                if (!ReferenceEquals(output, capture))
                    output.Dispose();
            }
        }

        var directory = Path.GetDirectoryName(DebugOutputPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(DebugOutputPath, bytes);
        Console.WriteLine($"screenshot returning image/png, bytes={bytes.Length}, debugCopy={DebugOutputPath}");

        return [ImageContentBlock.FromBytes(bytes, "image/png")];
    }
}
