# Python prompt_toolkit parity matrix

Last audit: 2026-05-11. This audit was produced by walking the C# source tree, the test suite, the example scenarios, and the CI pipeline, and comparing the implemented surface against [python-prompt-toolkit](https://github.com/prompt-toolkit/python-prompt-toolkit).

Legend: ✅ broad coverage of the upstream module, 🚧 partial coverage with stable APIs, ❌ not yet ported.

| Upstream module | C# namespace(s) | Status | Notes |
| --- | --- | --- | --- |
| `application` | `DotnetPromptToolkit.Application` | 🚧 | `PromptSession` with history, completer, validator, auto-suggest, editing mode, multiline. Full async event loop and key processor integration not yet wired. |
| `auto_suggest` | `DotnetPromptToolkit.AutoSuggest` | ✅ | `IAutoSuggest`, `Suggestion`, `DummyAutoSuggest`, `AutoSuggestFromHistory`, `ThreadedAutoSuggest`. |
| `buffer` / `document` | `DotnetPromptToolkit.Buffers` | 🚧 | `Document`, `Buffer` with insert/delete/cursor/undo/accept/validation/completion. Selection state ported in `Selection` namespace. Multiline editing helpers remain partial. |
| `cache` | `DotnetPromptToolkit.Cache` | ✅ | `SimpleCache<TKey,TValue>` (LRU) and `FastDictCache<TKey,TValue>`. |
| `clipboard` | `DotnetPromptToolkit.Clipboard` | ✅ | `IClipboard`, `InMemoryClipboard`, `KillRingClipboard` (rich kill-ring with rotation), `DynamicClipboard`, `ClipboardData`. |
| `completion` | `DotnetPromptToolkit.Completion` | ✅ | `ICompleter`, `Completion`, `CompletionState`, `WordCompleter`, `FuzzyWordCompleter`, `PathCompleter`, `NestedCompleter`, `MergedCompleter`, `DeduplicateCompleter`. |
| `contrib.regular_languages` | `DotnetPromptToolkit.Contrib.RegularLanguages` | 🚧 | `GrammarCompiler.Compile` produces a `CompiledGrammar` with named-group variable extraction. Grammar-driven completion not yet ported. |
| `cursor_shapes` | `DotnetPromptToolkit.CursorShapes` | ✅ | `CursorShape` enum, `ICursorShapeConfig`, `StaticCursorShape`, `DynamicCursorShapeConfig`. |
| `data_structures` | `DotnetPromptToolkit.DataStructures` | ✅ | `Point`, `Size` records. |
| `enums` | `DotnetPromptToolkit.Enums` | ✅ | `EditingMode`. |
| `eventloop` | `DotnetPromptToolkit.EventLoop` | 🚧 | `EventLoopHelpers.RunInBackgroundAsync`, `RunInExecutorAsync`, `RunInTerminalAsync`, `InputHook`/`InputHookContext`. Full async loop integration is future work. |
| `filters` | `DotnetPromptToolkit.Filters` | ✅ | `Filter` base with `&`, `|`, `~`, `Always`, `Never`, `Condition`, `From`. App helpers in `AppFilters` accept delegates. |
| `formatted_text` | `DotnetPromptToolkit.FormattedText` | ✅ | `FormattedText`, `FormattedTextFragment`, `HtmlFormattedText.Parse` (matches upstream `class:<tag>` style), `AnsiFormattedText.Parse` (SGR), `FormattedTextUtils.SplitLines`. |
| `history` | `DotnetPromptToolkit.History` | ✅ | `IHistory`, `InMemoryHistory`, `FileHistory` (round-trips multi-line entries), `ThreadedHistory`. |
| `input` (vt100) | `DotnetPromptToolkit.Input` | 🚧 | `AnsiInputParser` handles arrows, function keys (Home/End/Page/Insert/Delete), SS3 sequences, and the full Ctrl-A..Ctrl-Z range. Bracketed paste and mouse parsing remain. |
| `key_binding` | `DotnetPromptToolkit.KeyBinding` | 🚧 | `KeyBindings`, `KeyProcessor`, default editing handlers, `EmacsBindings.Create`, `ViBindings.Create`, `ViState`, `EmacsState`. Filter-based bindings and full Vi/Emacs keymaps remain. |
| `keys` | `DotnetPromptToolkit.Keys` | ✅ | `Keys` enum mirroring upstream identifiers, `KeyNames.Name` returns matching string ids (e.g. `c-a`, `s-tab`). |
| `layout.containers` | `DotnetPromptToolkit.Layout` | 🚧 | `HSplit`, `VSplit`, `Window`, `Float`, `FloatContainer`, `ScrollOffsets`. Conditional containers, alignments, paddings remain. |
| `layout.controls` | `DotnetPromptToolkit.Layout` | 🚧 | `IUIControl`, `FormattedTextControl`, `BufferControl`. Display-mapping for line wrap/scroll remains. |
| `layout.dimension` | `DotnetPromptToolkit.Layout` | ✅ | `Dimension`, `D` helper, `DimensionDistributor.Distribute`. |
| `layout.margins` | `DotnetPromptToolkit.Layout` | ✅ | `IMargin`, `NumberedMargin`, `ScrollbarMargin`. |
| `layout.menus` | `DotnetPromptToolkit.Layout` | ✅ | `CompletionsMenu` with selected-index highlighting. |
| `layout.processors` | `DotnetPromptToolkit.Layout` | 🚧 | `IProcessor`, `HighlightSearchProcessor`, `PasswordProcessor`. Full processor pipeline remains. |
| `layout.mouse_handlers` | `DotnetPromptToolkit.Layout` | 🚧 | `MouseHandler` delegate alias. Hit-testing and event routing remain. |
| `lexers` | `DotnetPromptToolkit.Lexers` | ✅ | `ILexer`, `SimpleLexer`, `DelegatingLexer` (per-line tokenizer). |
| `mouse_events` | `DotnetPromptToolkit.MouseEvents` | ✅ | `MouseEvent`, `MouseEventType`, `MouseButton`, `MouseModifier`. |
| `output` | `DotnetPromptToolkit.Output` | ✅ | `ITerminalOutput`, `ConsoleTerminalOutput`, `Vt100Output` with bracketed paste / alt-screen / cursor / colour, `PlainTextOutput`, `DummyOutput`, `ColorDepth`, `ColorDepthExtensions.FromEnvironment`, `Ansi` helper. |
| `patch_stdout` | `DotnetPromptToolkit.PatchStdout` | ✅ | `PatchStdoutScope` buffers `Console.Out` until disposed. |
| `renderer` | `DotnetPromptToolkit.Rendering` | 🚧 | `Screen`, `Renderer` produce diff operations; full screen/cursor diffing remains. |
| `search` | `DotnetPromptToolkit.Search` | ✅ | `Searcher.FindNext`, `SearchState`, `SearchDirection`, `IncrementalSearcher.Find` (forward/backward, case-insensitive). |
| `selection` | `DotnetPromptToolkit.Selection` | ✅ | `SelectionType`, `SelectionState`, `PasteMode`. |
| `shortcuts.prompt` | `DotnetPromptToolkit.Shortcuts` | 🚧 | `Prompt.RunAsync`, `Prompt.RunPasswordAsync`. Streaming completion menu UI remains. |
| `shortcuts.dialogs` | `DotnetPromptToolkit.Shortcuts` | ✅ | `Dialogs.MessageDialog`, `YesNoDialog`, `InputDialog`, `ButtonDialog`, `ChoiceDialog`. |
| `shortcuts.progress_bar` | `DotnetPromptToolkit.Shortcuts` | ✅ | `ProgressBar` with `Advance`/`Reset`/`Render`. |
| `styles` | `DotnetPromptToolkit.Styles` | ✅ | `Style`, `StyleRule`, `DefaultStyle`, `NamedColors`, `IStyleTransformation`, `IdentityStyleTransformation`, `SwapLightAndDarkStyleTransformation`, `ConditionalStyleTransformation`, `MergedStyleTransformation`. |
| `token` | `DotnetPromptToolkit.Token` | ✅ | `Token` records mirroring common Pygments token names. |
| `validation` | `DotnetPromptToolkit.Validation` | ✅ | `IValidator`, `ValidationError`, `DelegateValidator`, `DummyValidator`, `ThreadedValidator`, `WordValidator`, `RegexValidator`. |
| `widgets.base` | `DotnetPromptToolkit.Widgets` | ✅ | `TextLabel`, `Frame`, `Box`, `Button`, `Checkbox`, `RadioList<T>`, `FormattedTextToolbar`. |
| `widgets.dialogs` | `DotnetPromptToolkit.Widgets` | ✅ | `Dialog` widget rendered through `Frame`. |
| `widgets.menus` | `DotnetPromptToolkit.Widgets` | ✅ | `MenuItem`, `MenuContainer`. |

## Remaining gaps

The following upstream modules are not yet ported. They are tracked as future work:
- `input.win32` (Windows console reader)
- `input.vt100` mouse / bracketed paste decoding
- `output.win32` (Windows console writer)
- Full Vi/Emacs key binding catalogs and key chord sequences
- Complete renderer with screen diffing and cursor placement
- `patch_stdout` integration with the renderer
- `contrib.ssh`, `contrib.telnet`
- `pygments`-based lexer adapter

## Repeatable parity validation

Install the Python reference dependency once:

```bash
python -m pip install -r tools/python-requirements.txt
```

Run the deterministic Python-vs-C# parity scenarios as many times as you need:

```bash
python tools/validate_examples.py --iterations 3
```

Each iteration builds the .NET solution, executes every C# parity scenario, executes the matching Python `prompt_toolkit` reference scenario, and compares the JSON outputs for an exact match. Re-running the validator multiple times with the same iteration count is a fast and deterministic confidence check.

Validated scenarios (15):

- `document`
- `completion`
- `fuzzy-completion`
- `nested-completion`
- `history`
- `formatted-text`
- `html`
- `ansi`
- `layout`
- `search`
- `auto-suggest`
- `named-colors`
- `grammar`
- `progress-bar`
- `frame`

Run a single scenario with:

```bash
python tools/validate_examples.py --scenario document --iterations 5 --no-build
```
