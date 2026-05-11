using DotnetPromptToolkit.Buffers;

namespace DotnetPromptToolkit.Completion;

public sealed record Completion(string Text, int StartPosition = 0, string? Display = null, string? DisplayMetaText = null)
{
    public string Apply(Document document)
    {
        var start = Math.Clamp(document.CursorPosition + StartPosition, 0, document.Text.Length);
        return document.Text[..start] + Text + document.TextAfterCursor;
    }
}

public interface ICompleter
{
    ValueTask<IReadOnlyList<Completion>> GetCompletionsAsync(Document document, CancellationToken cancellationToken = default);
}

public sealed class WordCompleter(IEnumerable<string> words, bool ignoreCase = false, bool matchMiddle = false) : ICompleter
{
    private readonly List<string> _words = words.Distinct().Order(StringComparer.Ordinal).ToList();
    private readonly StringComparison _comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    public ValueTask<IReadOnlyList<Completion>> GetCompletionsAsync(Document document, CancellationToken cancellationToken = default)
    {
        var word = document.GetWordBeforeCursor();
        var completions = _words
            .Where(w => matchMiddle ? w.Contains(word, _comparison) : w.StartsWith(word, _comparison))
            .Select(w => new Completion(w, -word.Length))
            .ToArray();
        return ValueTask.FromResult<IReadOnlyList<Completion>>(completions);
    }
}
