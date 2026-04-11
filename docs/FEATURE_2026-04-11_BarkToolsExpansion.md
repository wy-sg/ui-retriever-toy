# Bark Tools Expansion — GetText & SetFocus

> Date: 2026-04-11

## Summary

Added two new bark operations (`get_text`, `set_focus`) to the Toy MCP server, completing the five core barks requested:

| MCP Tool | Bark | Arguments | Status |
|---|---|---|---|
| `get_text` | GetText | `markName` | **New** |
| `invoke` | Invoke | `markName` | Existing |
| `click_element` | ClickElement | `markName` | Existing |
| `set_text` | SetValue | `markName`, `value` | Existing |
| `set_focus` | SetFocus | `markName` | **New** |

## Changes

### UIRetriever.Contracts (1.0.2 → 1.0.3)

| File | Change |
|---|---|
| `Barks/GetTextRequest.cs` | **New** — request DTO for `get_text` |
| `Barks/SetFocusRequest.cs` | **New** — request DTO for `set_focus` |
| `ToolNames.cs` | Added `GetText` and `SetFocus` constants |

### UIRetriever.Toy.Core (→ 1.0.1)

| File | Change |
|---|---|
| `Services/IToyExecutor.cs` | Added `GetTextAsync`, `SetFocusAsync` |
| `Services/ToyExecutor.cs` | Added stub implementations (return `NotImplemented`) |

### UIRetriever.Toy.Mcp (1.0.8 → 1.0.9)

| File | Change |
|---|---|
| `Tools/BarkTools.cs` | Added `get_text` and `set_focus` MCP tools |

## Implementation Notes

- `get_text` resolves the mark, calls `IElementHandle.GetText()`, and returns the text or an error.
  The underlying FlaUI implementation tries Value pattern → Text pattern → element Name (matching legacy UiBridge `FlaUIElementHandle.GetText()`).
- `set_focus` resolves the mark and calls `IElementHandle.SetFocus()`, which maps to FlaUI's `Focus()` method (matching legacy UiBridge `FlaUIElementHandle.SetFocus()`).
- Both follow the same `ResolveMark` → handle-method pattern used by the existing barks.
