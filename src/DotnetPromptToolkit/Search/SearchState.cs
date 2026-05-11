using DotnetPromptToolkit.Buffers;

namespace DotnetPromptToolkit.Search;

/// <summary>Mirrors prompt_toolkit.search.SearchDirection.</summary>
public enum SearchDirection
{
    Forward,
    Backward
}

/// <summary>Mirrors prompt_toolkit.search.SearchState.</summary>
public sealed class SearchState
{
    public SearchState(string text = "", SearchDirection direction = SearchDirection.Forward, bool ignoreCase = false)
    {
        Text = text ?? string.Empty;
        Direction = direction;
        IgnoreCase = ignoreCase;
    }

    public string Text { get; set; }
    public SearchDirection Direction { get; set; }
    public bool IgnoreCase { get; set; }

    public SearchState Clone() => new(Text, Direction, IgnoreCase);
}

/// <summary>Searcher that supports forward and backward searching, mirroring upstream search semantics.</summary>
public static class IncrementalSearcher
{
    public static SearchResult? Find(Document document, SearchState state, int? startIndex = null)
    {
        if (string.IsNullOrEmpty(state.Text)) return null;
        var comparison = state.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (state.Direction == SearchDirection.Forward)
        {
            var from = Math.Clamp(startIndex ?? document.CursorPosition, 0, document.Text.Length);
            var index = document.Text.IndexOf(state.Text, from, comparison);
            return index < 0 ? null : new SearchResult(index, state.Text.Length);
        }
        else
        {
            var from = Math.Clamp(startIndex ?? document.CursorPosition, 0, document.Text.Length);
            var index = document.Text.LastIndexOf(state.Text, Math.Max(0, from - 1), comparison);
            return index < 0 ? null : new SearchResult(index, state.Text.Length);
        }
    }
}
