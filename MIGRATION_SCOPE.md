# UIRetriever.Toy Migration Scope

## Purpose

This document clarifies what is in scope for the public `uiretriever-toy` repo and what is intentionally excluded.

## In Scope

### Contracts (`UIRetriever.Contracts`)
- Bark request/response DTOs for: Invoke, SetText, ClickElement, RightClickElement
- Tool name constants
- Common result and error models
- Public-safe enums and shared contract types

### Schema (`UIRetriever.Schema`)
- Marks file schema models (MarksFile, MarkEntry, MarkNodeEntry)
- Schema version tracking
- Serialization placeholders
- Import/export placeholders

### Toy.Core (`UIRetriever.Toy.Core`)
- `IToyExecutor` service interface
- Minimal placeholder implementation
- Supports only: Invoke, SetText, ClickElement, RightClickElement
- No premium discovery, healing, or advanced logic

### Toy.Mcp (`UIRetriever.Toy.Mcp`)
- Console MCP host with bark tool registration
- Simple executor — no errands, tricks, or procedures
- References Contracts, Schema, and Toy.Core

## Intentionally Excluded

| Feature | Reason |
|---|---|
| Errand (session) support | Premium feature — private repo only |
| Trick (module) support | Premium feature — private repo only |
| Procedure support | Premium feature — private repo only |
| Context variables | Premium feature — private repo only |
| Premium selector healing | Premium feature — private repo only |
| Advanced screenshot-to-control mapping | Premium feature — private repo only |
| Hover-to-tree pickup logic | Premium feature — private repo only |
| Replay / diagnostics engines | Premium feature — private repo only |
| Licensing / premium gates | Private repo only |
| Park WPF app logic | Private repo only |
| Bookmarker MCP host | Private repo only |

## Relationship to Private Repo

- The private `uiretriever` repo contains the full product engine.
- Public contracts and schema models define the shapes that private implementations also use.
- The private repo does NOT depend on public Toy projects directly.
- If a concept must be shared, only the public-safe contract/schema shape lives here.
- Marks are created by Park / Park.Mcp (private) and consumed by executors.
- Toy.Mcp is the last/simplest executor — it is migrated after the private creator and executor paths are stable.
