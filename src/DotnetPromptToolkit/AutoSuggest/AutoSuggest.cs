using DotnetPromptToolkit.Buffers;
using DotnetPromptToolkit.History;

namespace DotnetPromptToolkit.AutoSuggest;

/// <summary>Mirrors prompt_toolkit.auto_suggest.Suggestion.</summary>
public sealed record Suggestion(string Text);

/// <summary>Mirrors prompt_toolkit.auto_suggest.AutoSuggest.</summary>
public interface IAutoSuggest
{
    ValueTask<Suggestion?> GetSuggestionAsync(Document document, CancellationToken cancellationToken = default);
}

/// <summary>Returns no suggestions, mirrors DummyAutoSuggest.</summary>
public sealed class DummyAutoSuggest : IAutoSuggest
{
    public ValueTask<Suggestion?> GetSuggestionAsync(Document document, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult<Suggestion?>(null);
}

/// <summary>Suggests text from history. Mirrors AutoSuggestFromHistory.</summary>
public sealed class AutoSuggestFromHistory(IHistory history) : IAutoSuggest
{
    public ValueTask<Suggestion?> GetSuggestionAsync(Document document, CancellationToken cancellationToken = default)
    {
        var line = document.CurrentLineBeforeCursor;
        if (string.IsNullOrEmpty(line)) return ValueTask.FromResult<Suggestion?>(null);

        // Walk history from newest to oldest.
        var entries = history.GetStrings();
        for (var i = entries.Count - 1; i >= 0; i--)
        {
            var entry = entries[i];
            if (entry.StartsWith(line, StringComparison.Ordinal) && entry.Length > line.Length)
            {
                return ValueTask.FromResult<Suggestion?>(new Suggestion(entry[line.Length..]));
            }
        }
        return ValueTask.FromResult<Suggestion?>(null);
    }
}

/// <summary>Wraps another auto-suggest and runs it on a thread pool, mirroring ThreadedAutoSuggest.</summary>
public sealed class ThreadedAutoSuggest(IAutoSuggest inner) : IAutoSuggest
{
    public ValueTask<Suggestion?> GetSuggestionAsync(Document document, CancellationToken cancellationToken = default) =>
        new(Task.Run(() => inner.GetSuggestionAsync(document, cancellationToken).AsTask(), cancellationToken));
}
