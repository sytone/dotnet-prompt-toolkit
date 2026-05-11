# dotnet-prompt-toolkit

A .NET port-in-progress of Python's `prompt_toolkit` concepts. The current implementation establishes the solution structure and foundational APIs for buffers, documents, completions, validation, history, key bindings, input parsing, formatted text, styles, layouts, rendering, terminal output, and prompt sessions.

## Build and test

```bash
dotnet build DotnetPromptToolkit.slnx
dotnet test DotnetPromptToolkit.slnx
```

## Examples

```bash
dotnet run --project examples/DotnetPromptToolkit.Examples -- basic
dotnet run --project examples/DotnetPromptToolkit.Examples -- completion
dotnet run --project examples/DotnetPromptToolkit.Examples -- layout
```

## Status

This repository is not yet at full Python `prompt_toolkit` parity. See [docs/PARITY.md](docs/PARITY.md) for the tracked parity matrix and remaining work.
