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
dotnet run --project examples/DotnetPromptToolkit.Examples -- parity document
```

## Repeatable Python/C# parity validation

Install the Python reference dependency and run the deterministic comparison scenarios:

```bash
python -m pip install -r tools/python-requirements.txt
python tools/validate_examples.py --iterations 3
```

The validator compares Python `prompt_toolkit` reference outputs with C# example outputs for document, completion, history, formatted-text, and layout scenarios.

## Status

This repository is not yet at full Python `prompt_toolkit` parity. See [docs/PARITY.md](docs/PARITY.md) for the tracked parity matrix, the latest audit, and remaining work.
