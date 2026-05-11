# dotnet-prompt-toolkit

AI-first bootstrap for a full-fidelity C# port of [python-prompt-toolkit](https://github.com/prompt-toolkit/python-prompt-toolkit).

## Current baseline

- `src/DotnetPromptToolkit`: initial library surface.
- `tests/DotnetPromptToolkit.Tests`: parity-oriented tests.
- `samples/EchoSample`: deterministic sample used for Python/C# side-by-side validation.

## Run tests

```bash
dotnet test DotnetPromptToolkit.slnx
```

## Run side-by-side sample check

```bash
dotnet run --project samples/EchoSample -- "> " "sample"
python3 -c 'import sys; p=sys.argv[1]; t=sys.argv[2]; print(f"{p}{t}")' "> " "sample"
```
