# Python prompt_toolkit parity matrix

Legend: ✅ initial support, 🚧 partial support, ❌ not yet ported.

| Area | Status | Notes |
| --- | --- | --- |
| Solution/library/test/example structure | ✅ | `DotnetPromptToolkit.slnx` with library, xUnit tests, and examples. |
| Document and buffer primitives | 🚧 | Text, cursor, line translation, word lookup, editing, undo, accept, validation hooks. |
| Completion | 🚧 | `ICompleter`, `Completion`, and `WordCompleter`; richer nested/path/fuzzy completions remain. |
| Validation | 🚧 | Validator interface and delegate validator; richer validation UX remains. |
| History | 🚧 | In-memory history; file/threaded history remains. |
| Clipboard | 🚧 | In-memory clipboard; system clipboard remains. |
| Search | 🚧 | Forward text search; incremental/reverse search remains. |
| Key bindings | 🚧 | Key processor, basic keys, custom bindings; filters, Vi/Emacs maps, macros remain. |
| Input backend | 🚧 | Basic ANSI parser; bracketed paste, mouse, CPR, full VT parsing remain. |
| Output backend | 🚧 | Console output and ANSI helpers; terminal capability negotiation remains. |
| Formatted text | 🚧 | Styled fragments and plain-text conversion; HTML/ANSI/Pygments-style helpers remain. |
| Styles | 🚧 | Selector lookup; cascading/class merging and color depth remain. |
| Layout/widgets | 🚧 | Label, text area, horizontal/vertical splits; full widget catalog remains. |
| Rendering | 🚧 | Screen model and simple render diff; full screen diffing/cursor/menu rendering remains. |
| Application/session async | 🚧 | Basic `PromptSession.PromptAsync`; full application event loop remains. |
| Python examples | 🚧 | Basic, completion, and layout examples added; all upstream examples remain. |
| Python tests | 🚧 | Initial translated-style tests added; full upstream suite remains. |
