namespace UIRetriever.Contracts;

/// <summary>
/// MCP tool name constants for UIRetriever operations.
/// </summary>
public static class ToolNames
{
    // Bark tools
    public const string Invoke = "invoke";
    public const string SetText = "set_text";
    public const string ClickElement = "click_element";
    public const string RightClickElement = "right_click_element";

    // Mark authoring tools
    public const string MarkElement = "mark_element";
    public const string ListMarks = "list_marks";
    public const string ValidateMark = "validate_mark";
    public const string DeleteMark = "delete_mark";

    // Utility tools
    public const string Screenshot = "screenshot";
    public const string GridedScreenshot = "grided_screenshot";
}
