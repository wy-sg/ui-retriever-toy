# UIRetriever.Toy — Copilot Instructions

## Product Identity

- **UIRetriever.Toy** is the public playground/open subset of the UIRetriever product family.
- This repo contains only public-safe contracts, schema models, and toy-level implementations.
- No premium logic. No hidden premium code paths.

## Project Roles

| Project | Role |
|---|---|
| `UIRetriever.Contracts` | Public DTOs and contract surface. Tool names, bark request/response types, result/error models. |
| `UIRetriever.Schema` | Marks file schema models. Schema version, serialization/validation placeholders. |
| `UIRetriever.Toy.Core` | Toy-safe minimal execution layer. Service interfaces and placeholder implementations for simple barks. |
| `UIRetriever.Toy.Mcp` | Public lite executor MCP host. Registers bark tools, maps requests to Toy.Core services. Simple executor only. |

## Domain Vocabulary

- **mark** = a saved path/selector to a UI element (was: bookmark)
- **bark** = an atomic operation/action (was: operation)
- **procedure** = a sequence of steps (unchanged — but NOT supported in Toy)
- **errand** = a runtime job/session (NOT supported in Toy)
- **trick** = a reusable module/capability (NOT supported in Toy)
- **context** = runtime variables (unchanged — but NOT supported in Toy)

## Architecture Rules

- No premium logic. Ever.
- Keep implementations simple and demonstrative.
- Contracts define bark shapes (Invoke, SetText, ClickElement, RightClickElement).
- Schema defines marks file/schema models.
- Toy.Core defines toy-safe service interfaces and minimal implementations.
- Toy.Mcp defines MCP tool mapping and host composition.
- Do NOT add modules, errands, tricks, or procedure features.
- Do NOT add hidden premium code paths behind flags.
- Do NOT add advanced selector healing, hover-to-tree pickup, or replay/diagnostics.

## Dependency Rules

- `UIRetriever.Contracts` — no project references
- `UIRetriever.Schema` — no project references
- `UIRetriever.Toy.Core` → `UIRetriever.Contracts` + `UIRetriever.Schema`
- `UIRetriever.Toy.Mcp` → `UIRetriever.Contracts` + `UIRetriever.Schema` + `UIRetriever.Toy.Core`

## What Toy.Mcp Supports

- Simple bark execution: Invoke, SetText, ClickElement, RightClickElement
- Marks loaded from a marks file
- No errands, no tricks, no procedures, no sessions/tasks
