using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using ModelContextProtocol.Server;
using UIRetriever.Core.Marks;
using UIRetriever.Windows;

namespace UIRetriever.Toy.Mcp.Tools;

[McpServerToolType]
public sealed class BarkTools
{
    private static readonly string MarksFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "UIRetriever", "marks.json");

    [McpServerTool(Name = "invoke")]
    [Description("Invoke (activate/press) a marked UI element.")]
    public static string Invoke(
        [Description("Name of the mark identifying the target element")] string markName)
    {
        var (handle, error) = ResolveMark(markName);
        if (handle is null) return error!;
        return handle.Invoke()
            ? $"Invoked '{markName}'."
            : $"Error: invoke failed for '{markName}'.";
    }

    [McpServerTool(Name = "set_text")]
    [Description("Set the text/value of a marked UI element.")]
    public static string SetText(
        [Description("Name of the mark identifying the target element")] string markName,
        [Description("The text value to set")] string value)
    {
        var (handle, error) = ResolveMark(markName);
        if (handle is null) return error!;
        return handle.SetValue(value)
            ? $"Set text on '{markName}'."
            : $"Error: set_text failed for '{markName}'.";
    }

    [McpServerTool(Name = "click_element")]
    [Description("Click a marked UI element.")]
    public static string ClickElement(
        [Description("Name of the mark identifying the target element")] string markName)
    {
        var (handle, error) = ResolveMark(markName);
        if (handle is null) return error!;
        return handle.Click()
            ? $"Clicked '{markName}'."
            : $"Error: click failed for '{markName}'.";
    }

    [McpServerTool(Name = "right_click_element")]
    [Description("Right-click a marked UI element.")]
    public static string RightClickElement(
        [Description("Name of the mark identifying the target element")] string markName)
    {
        var (handle, error) = ResolveMark(markName);
        if (handle is null) return error!;

        var info = handle.GetInfo();
        var b = info.Bounds;
        if (b.Width <= 0 || b.Height <= 0)
            return $"Error: element '{markName}' has no valid bounds for right-click.";

        int cx = b.X + b.Width / 2;
        int cy = b.Y + b.Height / 2;
        SetCursorPos(cx, cy);
        mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, cx, cy, 0, 0);
        return $"Right-clicked '{markName}'.";
    }

    private static (UIRetriever.Core.Services.IElementHandle? Handle, string? Error) ResolveMark(string markName)
    {
        var fileService = new MarkFileService();
        var marks = fileService.Load(MarksFilePath);
        var mark = marks.FirstOrDefault(m => string.Equals(m.Name, markName, StringComparison.OrdinalIgnoreCase));

        if (mark is null)
            return (null, $"Error: no mark named '{markName}' found.");

        var resolver = new MarkResolver();
        var (handle, trace) = resolver.ResolveToHandle(mark);

        if (handle is null || !handle.IsValid)
            return (null, $"Error: could not resolve '{markName}'.\nTrace:\n{trace}");

        return (handle, null);
    }

    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
}
