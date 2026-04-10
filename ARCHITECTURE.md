# UIRetriever.Toy Architecture

## Overview

UIRetriever.Toy is the public playground subset of the UIRetriever product family. It provides a minimal, demonstrative set of UI automation contracts and a simple executor MCP host.

## Repo Split

| Repo | Visibility | Purpose |
|---|---|---|
| `uiretriever` | Private | Paid product: full engine, WPF app, executor MCP, bookmarker MCP |
| `uiretriever-toy` (this repo) | Public | Playground: contracts, schema, toy executor MCP |

The split exists to keep premium logic private while offering a public playground for simple automation and community experimentation.

## Projects

### UIRetriever.Contracts

Public DTOs and contract surface.

- Tool name constants
- Bark request types (Invoke, SetText, ClickElement, RightClickElement)
- Result and error models
- No business logic

### UIRetriever.Schema

Marks file schema models.

- `MarksFile` — root container for a marks file
- `MarkEntry` — a single mark definition
- `MarkNodeEntry` — a node in the mark's element chain
- Schema version tracking
- Serialization/validation placeholders

### UIRetriever.Toy.Core

Toy-safe minimal execution layer.

- `IToyExecutor` — interface for executing simple barks
- `ToyExecutor` — placeholder implementation
- No premium features (no errands, tricks, procedures, selector healing)

### UIRetriever.Toy.Mcp

Public lite executor MCP host (console app).

- Registers bark tools mapped from Contracts
- Dispatches to Toy.Core services
- JSON-RPC 2.0 over stdio
- Simple executor only — no errands, tricks, or procedures

## Dependency Graph

```
UIRetriever.Contracts    UIRetriever.Schema
         ↑                    ↑
         └────────┬───────────┘
                  ↓
         UIRetriever.Toy.Core
                  ↑
         UIRetriever.Toy.Mcp
```

## What Is Intentionally Out of Scope

- Premium selector healing
- Advanced screenshot-to-control mapping
- Hover-to-tree pickup logic
- Replay / diagnostics engines
- Licensing / premium gates
- Errand (session) support
- Trick (module) support
- Procedure support
- Context variable support
