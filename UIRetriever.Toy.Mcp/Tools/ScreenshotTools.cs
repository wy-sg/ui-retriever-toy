using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    [Description("Take a screenshot of the entire primary screen and return it as a JPEG image for LLM analysis. " +
                 "The image is scaled down to keep the payload small.")]
    public static IList<AIContent> TakeScreenshot(
        [Description("Maximum width in pixels (default 1280). Height is scaled proportionally.")] int maxWidth = 1280,
        [Description("JPEG quality 1-100 (default 70). Lower = smaller file.")] int quality = 70)
    {
        quality = Math.Clamp(quality, 1, 100);
        maxWidth = Math.Clamp(maxWidth, 320, 3840);

        var screen = Screen.PrimaryScreen!;
        var bounds = screen.Bounds;

        using var capture = new Bitmap(bounds.Width, bounds.Height);
        using (var g = Graphics.FromImage(capture))
            g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);

        // Scale down if wider than maxWidth
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
            var encoder = ImageCodecInfo.GetImageEncoders().First(e => e.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParams = new EncoderParameters(1)
            {
                Param = { [0] = new EncoderParameter(Encoder.Quality, (long)quality) }
            };
            output.Save(ms, encoder, encoderParams);

            return [new DataContent(ms.ToArray(), "image/jpeg")];
        }
        finally
        {
            if (!ReferenceEquals(output, capture))
                output.Dispose();
        }
    }
}
