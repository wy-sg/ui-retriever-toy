using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using ModelContextProtocol.Server;
using UIRetriever.Bridge;

namespace UIRetriever.Toy.Mcp.Tools;

[McpServerToolType]
public sealed class MarkTools
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);
    private static readonly string MarksFilePath = InitMarksFilePath();

    private static string InitMarksFilePath()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "UIRetriever", "marks.json");
        var dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!File.Exists(path))
            File.WriteAllText(path, "[]");
        return path;
    }

    [McpServerTool(Name = "mark_element")]
    [Description("Mark a UI element and save it by name. " +
                 "Call with ONLY the name parameter to pick the element currently under the user's mouse cursor — this is the default and most common usage. " +
                 "Do NOT ask the user for coordinates or a screenshot; just call this tool with the name. " +
                 "Optionally, x and y can be provided to move the cursor to a specific screen coordinate before picking (the delay is then skipped).")]
    public static async Task<string> MarkElement(
        [Description("Unique name for the new mark. This is the only required parameter.")] string name,
        [Description("Delay in milliseconds before picking (default 1500). Ignored when x/y are provided.")] int delay = 1500,
        [Description("Optional screen X coordinate to move the cursor to before picking. Must be used together with y.")] int? x = null,
        [Description("Optional screen Y coordinate to move the cursor to before picking. Must be used together with x.")] int? y = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Error: name is required.";

        if (x.HasValue != y.HasValue)
            return "Error: both x and y must be provided together.";

        var marks = UIRetrieverEngine.LoadMarks(MarksFilePath);

        if (marks.Any(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)))
            return $"Error: a mark named '{name}' already exists.";

        if (x.HasValue && y.HasValue)
        {
            SetCursorPos(x.Value, y.Value);
        }
        else
        {
            await Task.Delay(delay);
        }

        var pickResult = UIRetrieverEngine.PickUnderCursor();

        if (!pickResult.Success || pickResult.Mark is null)
            return $"Error: pick failed — {pickResult.Message}";

        var mark = pickResult.Mark;
        mark.Name = name;
        mark.ProcessHint = pickResult.ProcessName ?? string.Empty;

        var tuneResult = UIRetrieverEngine.TuneMark(mark);

        if (!tuneResult.Success)
        {
            return $"Error: tuning failed at step [{tuneResult.FailedNodeIndex}].\n{tuneResult.Summary}";
        }

        var (handle, _) = UIRetrieverEngine.ResolveToHandle(mark);

        if (handle is null || !handle.IsValid)
        {
            return $"Error: tuning reported success but final resolve found no element.\n{tuneResult.Summary}";
        }

        var info = handle.GetInfo();
        var b = info.Bounds;
        bool hitTest = pickResult.CursorX >= b.X && pickResult.CursorX <= b.X + b.Width
                    && pickResult.CursorY >= b.Y && pickResult.CursorY <= b.Y + b.Height;

        if (!hitTest)
        {
            return $"Error: tuned element does not contain the pick point ({pickResult.CursorX},{pickResult.CursorY}). "
                 + $"Element bounds: ({b.X},{b.Y},{b.Width}x{b.Height}).\n{tuneResult.Summary}";
        }

        marks.Add(mark);
        UIRetrieverEngine.SaveMarks(marks, MarksFilePath);

        var excluded = tuneResult.Steps.Where(s => !s.Success).ToList();
        var summary = $"Marked '{name}' — {mark.Chain.Count} nodes, "
                    + $"{mark.Chain.Count(n => n.Include)} included, "
                    + $"element: Name='{info.Name}', Class='{info.ClassName}', AutoId='{info.AutomationId}'.";

        if (excluded.Count > 0)
            summary += $" ({excluded.Count} step(s) excluded during tuning)";

        return summary;
    }

    [McpServerTool(Name = "list_marks")]
    [Description("List all saved marks from the marks store.")]
    public static string ListMarks()
    {
        var marks = UIRetrieverEngine.LoadMarks(MarksFilePath);

        if (marks.Count == 0)
            return "No marks saved.";

        var lines = marks.Select((m, i) =>
            $"[{i}] {m.Name} — {m.Chain.Count} nodes, process: {m.ProcessHint}");

        return string.Join("\n", lines);
    }

    [McpServerTool(Name = "validate_mark")]
    [Description("Resolve a saved mark and report whether it finds the target element.")]
    public static string ValidateMark(
        [Description("Name of the mark to validate")] string name)
    {
        var marks = UIRetrieverEngine.LoadMarks(MarksFilePath);
        var mark = marks.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));

        if (mark is null)
            return $"Error: no mark named '{name}' found.";

        var (handle, trace) = UIRetrieverEngine.ResolveToHandle(mark);

        if (handle is null || !handle.IsValid)
            return $"Failed: could not resolve '{name}'.\nTrace:\n{trace}";

        var info = handle.GetInfo();
        return $"OK: '{name}' resolved — Name='{info.Name}', Class='{info.ClassName}', "
             + $"AutoId='{info.AutomationId}', Bounds=({info.Bounds.X},{info.Bounds.Y},{info.Bounds.Width}x{info.Bounds.Height}).";
    }

    [McpServerTool(Name = "delete_mark")]
    [Description("Delete a saved mark by name.")]
    public static string DeleteMark(
        [Description("Name of the mark to delete")] string name)
    {
        var marks = UIRetrieverEngine.LoadMarks(MarksFilePath);
        var removed = marks.RemoveAll(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));

        if (removed == 0)
            return $"Error: no mark named '{name}' found.";

        UIRetrieverEngine.SaveMarks(marks, MarksFilePath);
        return $"Deleted '{name}'.";
    }
}
