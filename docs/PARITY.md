# Python prompt_toolkit parity matrix

Last audit: 2026-05-11. This audit covers every source, test, example, and CI file currently in this repository and compares the implemented functionality with Python `prompt_toolkit` at a feature-area level.

Legend: ✅ initial support, 🚧 partial support, ❌ not yet ported.

| Area | Status | Current .NET coverage | Remaining Python prompt_toolkit parity gaps | Test/validation status |
| --- | --- | --- | --- | --- |
| Solution/library/test/example structure | ✅ | `DotnetPromptToolkit.slnx` with library, xUnit tests, examples, and CI. | Multi-target/package metadata and platform matrix are not defined. | `dotnet test DotnetPromptToolkit.slnx`; CI runs restore/build/test. |
| Document primitives | 🚧 | Immutable text, cursor clamping, line splitting, row/column translation, current-line helpers, word-before-cursor. | Selection state, find boundaries, word-under-cursor variants, cursor movement helpers, empty-line edge cases, grapheme/width-aware indexing. | xUnit coverage plus Python/C# parity scenario `document`. |
| Buffer/editing primitives | 🚧 | Insert, delete-before-cursor, cursor left/right, undo stack, accept with validation/history. | Full editing commands, multiline editing, selection, completion state, history navigation, validation display, filters, read-only buffers. | xUnit only; not yet covered by example parity except history acceptance. |
| Completion | 🚧 | `ICompleter`, `Completion`, `WordCompleter`, replacement via `StartPosition`. | Nested/path/fuzzy/deduplicated/threaded completions, completion menus, metadata formatting, async cancellation semantics matching upstream. | xUnit coverage plus Python/C# parity scenario `completion`. |
| Validation | 🚧 | `IValidator`, `DelegateValidator`, `ValidationError` with cursor position. | Validation processors, UI integration, async validation flows, validation state rendering. | xUnit only. |
| History | 🚧 | `InMemoryHistory` append/get strings. | File history, threaded history loading, duplicate policies, history search/navigation. | xUnit coverage plus Python/C# parity scenario `history`. |
| Clipboard | 🚧 | In-memory string clipboard. | System clipboard, multiple clipboard data types, integration with key bindings. | No dedicated parity scenario yet. |
| Search | 🚧 | Forward `IndexOf` search from cursor/start index. | Incremental search UI, reverse search, ignore-case defaults, multiline search behavior, search state. | xUnit only. |
| Key bindings | 🚧 | Basic key enum, key press model, registry, default character/backspace/left/right processing. | Full key grammar, key chords, filters, Vi/Emacs bindings, macros, conditional bindings, abort/accept flows. | xUnit only. |
| Input backend | 🚧 | Basic ANSI arrow parsing, enter, backspace, control-C/D, escape. | Full VT parser, bracketed paste, mouse, CPR, terminal modes, Windows console behavior. | xUnit only. |
| Output backend | 🚧 | Console output wrapper and minimal ANSI helpers. | Terminal capability detection, color depth, alternate screen, raw/cooked modes, cursor visibility, platform differences. | No dedicated parity scenario yet. |
| Formatted text | 🚧 | Styled fragments and plain-text conversion. | HTML/ANSI/Pygments helpers, fragment processors, style transformation, width-aware formatting. | xUnit coverage plus Python/C# parity scenario `formatted-text`. |
| Styles | 🚧 | Exact selector lookup from dictionary/rules. | Cascading selectors, class merging, defaults, color parsing, style transformations. | xUnit only. |
| Layout/widgets | 🚧 | `Label`, `TextArea`, `HSplit`, `VSplit` with plain-text rendering. | Full container system, dimensions, windows, controls, margins, floats, dialogs, menus, widgets, conditional containers. | xUnit coverage plus Python/C# parity scenario `layout`. |
| Rendering | 🚧 | `Screen` cell storage and simple renderer that emits all changed lines after plain-text comparison. | Real screen diffing, cursor placement, style rendering, invalidation, scroll offsets, mouse handlers. | xUnit only. |
| Application/session async | 🚧 | `PromptSession.PromptAsync` wrapping `Console.In.ReadLineAsync`. | Application event loop, input hooks, suspend-to-background, prompt rendering, bottom toolbars, completions, cancellation/interrupt semantics. | Manual examples only. |
| Examples | 🚧 | C# `basic`, `completion`, `layout`, plus noninteractive `parity` scenarios. | The upstream Python example catalog is not recreated. Interactive examples do not yet exercise real completion UI. | `tools/validate_examples.py` compares deterministic Python and C# scenario outputs. |
| Tests | 🚧 | xUnit tests for initial core behavior. | Upstream Python unit tests are not translated; platform/rendering/async regression coverage is minimal. | `dotnet test` and parity validator. |

## Repeatable parity validation

Install the Python reference dependency once:

```bash
python -m pip install -r tools/python-requirements.txt
```

Run the deterministic Python-vs-C# example parity scenarios:

```bash
python tools/validate_examples.py --iterations 3
```

The validator builds the .NET solution, runs each deterministic C# parity scenario, runs the matching Python `prompt_toolkit` reference scenario, compares the JSON outputs, and repeats the process for the requested number of iterations.

Current scenarios: `document`, `completion`, `history`, `formatted-text`, and `layout`.
