using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using DotnetPromptToolkit.AutoSuggest;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.Enums;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Validation;

namespace DotnetPromptToolkit.Application;

/// <summary>
/// Mirrors a subset of prompt_toolkit.shortcuts.PromptSession. Provides the
/// configuration knobs upstream offers (history, completer, validator,
/// auto-suggest, editing mode, multiline) and a non-interactive PromptAsync.
/// </summary>
public sealed class PromptSession
{
    public PromptSession(
        IHistory? history = null,
        ICompleter? completer = null,
        IValidator? validator = null,
        IAutoSuggest? autoSuggest = null,
        EditingMode editingMode = EditingMode.Emacs,
        bool multiline = false)
    {
        Buffer = new PromptBuffer(history: history, completer: completer, validator: validator);
        AutoSuggest = autoSuggest;
        EditingMode = editingMode;
        Multiline = multiline;
    }

    public PromptBuffer Buffer { get; }
    public IAutoSuggest? AutoSuggest { get; set; }
    public EditingMode EditingMode { get; set; }
    public bool Multiline { get; set; }

    public async ValueTask<string> PromptAsync(string message = "", CancellationToken cancellationToken = default)
    {
        Console.Write(message);
        var line = await Console.In.ReadLineAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
        Buffer.Reset(line);
        Buffer.Accept();
        return line;
    }
}
