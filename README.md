# UIRetriever.Toy

**UIRetriever.Toy** is the public playground of the UIRetriever product family — a lightweight [MCP](https://modelcontextprotocol.io/) server that lets AI agents interact with Windows desktop UI elements.


## Projects

| Project | Description |
|---|---|
| `UIRetriever.Contracts` | Public DTOs and contract surface — tool names, bark request/response types, result and error models. |
| `UIRetriever.Schema` | Marks-file schema models — schema version, serialization, and validation placeholders. |
| `UIRetriever.Toy.Core` | Toy-safe minimal execution layer — service interfaces and placeholder implementations. |
| `UIRetriever.Toy.Mcp` | MCP server host — registers bark tools, maps requests to Toy.Core services. Streamable HTTP on port 8000. |

## Vocabulary

| Term | Meaning |
|---|---|
| **mark** | A saved path/selector to a UI element. |
| **bark** | An atomic operation/action on a UI element. |

## Supported Tools

### Bark Tools
- `invoke` — Invoke/activate a marked element.
- `set_text` — Set text on a marked element.
- `click_element` — Click a marked element.
- `get_text` — Get text from a marked element.
- `set_focus` — Set focus on a marked element.

### Mark Authoring Tools
- `mark_element` — Pick, tune, and save a UI element as a named mark.
- `list_marks` — List all saved marks.
- `validate_mark` — Resolve a mark and report success/failure.
- `delete_mark` — Delete a saved mark.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/) (Windows)
- Windows 10 19041 or later

## Getting Started

```bash
# Clone the repo
git clone https://github.com/wysg/UIRetriever.Toy.git
cd UIRetriever.Toy

# Build
dotnet build

# Run the MCP server
dotnet run --project UIRetriever.Toy.Mcp
```

The server starts on `http://localhost:8000` using MCP Streamable HTTP transport.

