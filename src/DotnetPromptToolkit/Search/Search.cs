using DotnetPromptToolkit.Buffers;

namespace DotnetPromptToolkit.Search;

public sealed record SearchResult(int Index, int Length);

public static class Searcher
{
    public static SearchResult? FindNext(Document document, string query, int? startIndex = null, StringComparison comparison = StringComparison.Ordinal)
    {
        if (string.IsNullOrEmpty(query)) return null;
        var index = document.Text.IndexOf(query, Math.Clamp(startIndex ?? document.CursorPosition, 0, document.Text.Length), comparison);
        return index < 0 ? null : new SearchResult(index, query.Length);
    }
}
