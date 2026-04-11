using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace UIRetriever.Toy.Mcp.Tools;

[McpServerToolType]
public sealed class ScreenshotTools
{
    private const string DebugOutputPath = @"C:\temp\mcp_exact_return.png";
    internal const string Step1ImagePath = @"C:\temp\mcp_step1_direct.png";
    internal const string Step2ImagePath = @"C:\temp\mcp_step2_http.png";
    internal const string Step3ImagePath = @"C:\temp\mcp_step3_client.png";

    [McpServerTool(Name = "screenshot")]
    [Description("Take a screenshot of the entire primary screen and return it as a PNG image for LLM analysis. " +
                 "The image is scaled down to keep the payload small, and the exact returned bytes are also written to disk for diagnostics.")]
    public static IList<ContentBlock> TakeScreenshot(
        [Description("Maximum width in pixels (default 1280). Height is scaled proportionally.")] int maxWidth = 1280,
        [Description("Legacy JPEG quality parameter kept for compatibility. Ignored in PNG diagnostic mode.")] int quality = 70,
        [Description("Optional absolute path to a known-good PNG file to return instead of capturing the screen.")] string? pngFilePath = null,
        [Description("Optional absolute path where the returned PNG bytes should also be saved on the server.")] string? saveFilePath = null)
    {
        _ = Math.Clamp(quality, 1, 100);
        maxWidth = Math.Clamp(maxWidth, 320, 3840);

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
                bytes = CaptureScreenshotBytes(maxWidth);
            }
        }
        else
        {
            bytes = CaptureScreenshotBytes(maxWidth);
        }

        var directory = Path.GetDirectoryName(DebugOutputPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(DebugOutputPath, bytes);
        SaveArtifact(Step1ImagePath, bytes, "step1-direct");
        Console.WriteLine($"screenshot returning image/png, bytes={bytes.Length}, debugCopy={DebugOutputPath}");

        if (!string.IsNullOrWhiteSpace(saveFilePath))
        {
            try
            {
                SaveArtifact(saveFilePath, bytes, "custom-save");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save screenshot to {saveFilePath}: {ex.Message}");
            }
        }

        return [ImageContentBlock.FromBytes(bytes, "image/png")];
    }

    private static byte[] CaptureScreenshotBytes(int maxWidth)
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
            return ms.ToArray();
        }
        finally
        {
            if (!ReferenceEquals(output, capture))
                output.Dispose();
        }
    }

    internal static void SaveArtifact(string imagePath, byte[] bytes, string label)
    {
        var directory = Path.GetDirectoryName(imagePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(imagePath, bytes);

        var hashPath = GetHashPath(imagePath);
        var hash = ComputeSha256(bytes);
        File.WriteAllText(hashPath, hash + Environment.NewLine);

        Console.WriteLine($"{label}: file={imagePath}, bytes={bytes.Length}, sha256={hash}");
    }

    internal static string ComputeSha256(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));

    internal static string GetHashPath(string imagePath) => Path.ChangeExtension(imagePath, ".sha256.txt");
}
