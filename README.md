# dotnet-prompt-toolkit

A .NET port of [python-prompt-toolkit](https://github.com/prompt-toolkit/python-prompt-toolkit) to C#. Provides building blocks for building interactive command-line applications: documents, buffers, completion, validation, history, key bindings, input parsing, formatted text, styles, layouts, rendering, terminal output, prompt sessions, shortcuts, dialogs, progress bars, filters, key processors, and more.

## Build and test

```bash
dotnet build DotnetPromptToolkit.slnx
dotnet test DotnetPromptToolkit.slnx
```

## Examples

Interactive and deterministic example scenarios:

```bash
dotnet run --project examples/DotnetPromptToolkit.Examples -- basic
dotnet run --project examples/DotnetPromptToolkit.Examples -- completion
dotnet run --project examples/DotnetPromptToolkit.Examples -- layout
dotnet run --project examples/DotnetPromptToolkit.Examples -- parity all
```

The `samples/EchoSample` is a tiny deterministic side-by-side sample:

```bash
dotnet run --project samples/EchoSample -- "> " "sample"
```

## Repeatable Python/C# parity validation

Install the Python reference dependency once and run the deterministic comparison scenarios as many times as you like:

```bash
python -m pip install -r tools/python-requirements.txt
python tools/validate_examples.py --iterations 3
```

The validator compares Python `prompt_toolkit` reference outputs with C# example outputs across every parity scenario. See [docs/PARITY.md](docs/PARITY.md) for the full parity matrix and the list of validated scenarios.

## Status

This project tracks parity with `python-prompt-toolkit` module by module. See [docs/PARITY.md](docs/PARITY.md) for current coverage and remaining work.
