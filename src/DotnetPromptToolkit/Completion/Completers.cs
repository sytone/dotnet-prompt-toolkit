using DotnetPromptToolkit.Buffers;

namespace DotnetPromptToolkit.Completion;

/// <summary>
/// Mirrors prompt_toolkit.completion.fuzzy_completer.FuzzyWordCompleter. Performs
/// case-insensitive subsequence matching with ranked results.
/// </summary>
public sealed class FuzzyWordCompleter(IEnumerable<string> words) : ICompleter
{
    private readonly List<string> _words = words.Distinct().ToList();

    public ValueTask<IReadOnlyList<Completion>> GetCompletionsAsync(Document document, CancellationToken cancellationToken = default)
    {
        var word = document.GetWordBeforeCursor();
        if (string.IsNullOrEmpty(word))
        {
            IReadOnlyList<Completion> all = _words.Select(w => new Completion(w, 0)).ToArray();
            return ValueTask.FromResult(all);
        }

        var pattern = word.ToLowerInvariant();
        var ranked = _words
            .Select(w => (Word: w, Score: FuzzyScore(pattern, w.ToLowerInvariant())))
            .Where(t => t.Score >= 0)
            .OrderBy(t => t.Score)
            .ThenBy(t => t.Word, StringComparer.Ordinal)
            .Select(t => new Completion(t.Word, -word.Length))
            .ToArray();
        return ValueTask.FromResult<IReadOnlyList<Completion>>(ranked);
    }

    /// <summary>Returns -1 when no subsequence match, else lower is better.</summary>
    private static int FuzzyScore(string needle, string haystack)
    {
        var i = 0;
        var lastIndex = -1;
        var firstIndex = -1;
        foreach (var c in needle)
        {
            var found = haystack.IndexOf(c, i);
            if (found < 0) return -1;
            if (firstIndex < 0) firstIndex = found;
            lastIndex = found;
            i = found + 1;
        }
        return (lastIndex - firstIndex) + firstIndex;
    }
}

/// <summary>
/// Mirrors prompt_toolkit.completion.PathCompleter. Completes filesystem paths.
/// </summary>
public sealed class PathCompleter(bool onlyDirectories = false, string? rootPath = null) : ICompleter
{
    private readonly bool _onlyDirectories = onlyDirectories;
    private readonly string _rootPath = rootPath ?? Directory.GetCurrentDirectory();

    public ValueTask<IReadOnlyList<Completion>> GetCompletionsAsync(Document document, CancellationToken cancellationToken = default)
    {
        var text = document.TextBeforeCursor;
        var trailing = ExtractPath(text);
        var (dir, prefix) = SplitDirectoryPrefix(trailing, _rootPath);
        var results = new List<Completion>();
        try
        {
            if (Directory.Exists(dir))
            {
                IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(dir);
                foreach (var entry in entries)
                {
                    var name = Path.GetFileName(entry);
                    if (!name.StartsWith(prefix, StringComparison.Ordinal)) continue;
                    if (_onlyDirectories && !Directory.Exists(entry)) continue;
                    results.Add(new Completion(name, -prefix.Length, name + (Directory.Exists(entry) ? "/" : "")));
                }
            }
        }
        catch (UnauthorizedAccessException) { /* ignore */ }
        catch (IOException) { /* ignore */ }

        results.Sort((a, b) => string.CompareOrdinal(a.Text, b.Text));
        return ValueTask.FromResult<IReadOnlyList<Completion>>(results);
    }

    private static string ExtractPath(string text)
    {
        var i = text.Length;
        while (i > 0 && !char.IsWhiteSpace(text[i - 1])) i--;
        return text[i..];
    }

    private static (string Directory, string Prefix) SplitDirectoryPrefix(string path, string root)
    {
        if (string.IsNullOrEmpty(path)) return (root, string.Empty);
        var lastSep = path.LastIndexOfAny(new[] { '/', '\\' });
        if (lastSep < 0) return (root, path);
        var dir = path[..(lastSep + 1)];
        var prefix = path[(lastSep + 1)..];
        if (!Path.IsPathRooted(dir)) dir = Path.Combine(root, dir);
        return (dir, prefix);
    }
}

/// <summary>
/// Mirrors prompt_toolkit.completion.NestedCompleter. Routes completions based
/// on the first word, recursively into nested completers.
/// </summary>
public sealed class NestedCompleter : ICompleter
{
    private readonly Dictionary<string, ICompleter?> _options = new(StringComparer.Ordinal);

    public NestedCompleter() { }

    public NestedCompleter Add(string keyword, ICompleter? subCompleter)
    {
        _options[keyword] = subCompleter;
        return this;
    }

    public static NestedCompleter FromNestedDict(IReadOnlyDictionary<string, object?> data)
    {
        var completer = new NestedCompleter();
        foreach (var (key, value) in data)
        {
            ICompleter? sub = value switch
            {
                null => null,
                IEnumerable<string> words => new WordCompleter(words),
                IReadOnlyDictionary<string, object?> nested => FromNestedDict(nested),
                ICompleter c => c,
                _ => null
            };
            completer.Add(key, sub);
        }
        return completer;
    }

    public async ValueTask<IReadOnlyList<Completion>> GetCompletionsAsync(Document document, CancellationToken cancellationToken = default)
    {
        var text = document.TextBeforeCursor;
        var trimmed = text.TrimStart();
        var firstSpace = trimmed.IndexOf(' ');
        if (firstSpace < 0)
        {
            var prefix = trimmed;
            return _options.Keys
                .Where(k => k.StartsWith(prefix, StringComparison.Ordinal))
                .OrderBy(k => k, StringComparer.Ordinal)
                .Select(k => new Completion(k, -prefix.Length))
                .ToArray();
        }
        var first = trimmed[..firstSpace];
        if (_options.TryGetValue(first, out var sub) && sub is not null)
        {
            var rest = trimmed[(firstSpace + 1)..];
            var subDoc = new Document(rest, rest.Length);
            return await sub.GetCompletionsAsync(subDoc, cancellationToken).ConfigureAwait(false);
        }
        return Array.Empty<Completion>();
    }
}

/// <summary>Merges multiple completers, mirroring MergedCompleter.</summary>
public sealed class MergedCompleter(IEnumerable<ICompleter> completers) : ICompleter
{
    private readonly ICompleter[] _completers = completers.ToArray();

    public async ValueTask<IReadOnlyList<Completion>> GetCompletionsAsync(Document document, CancellationToken cancellationToken = default)
    {
        var results = new List<Completion>();
        foreach (var completer in _completers)
        {
            results.AddRange(await completer.GetCompletionsAsync(document, cancellationToken).ConfigureAwait(false));
        }
        return results;
    }
}

/// <summary>Removes duplicate completions by Text, mirroring DeduplicateCompleter.</summary>
public sealed class DeduplicateCompleter(ICompleter inner) : ICompleter
{
    public async ValueTask<IReadOnlyList<Completion>> GetCompletionsAsync(Document document, CancellationToken cancellationToken = default)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var deduped = new List<Completion>();
        foreach (var completion in await inner.GetCompletionsAsync(document, cancellationToken).ConfigureAwait(false))
        {
            if (seen.Add(completion.Text)) deduped.Add(completion);
        }
        return deduped;
    }
}

/// <summary>State of an active completion menu, mirroring CompletionState.</summary>
public sealed class CompletionState
{
    public CompletionState(int originalDocumentCursor, IReadOnlyList<Completion> completions)
    {
        OriginalDocumentCursor = originalDocumentCursor;
        Completions = completions;
        Complete_Index = completions.Count == 0 ? null : 0;
    }

    public int OriginalDocumentCursor { get; }
    public IReadOnlyList<Completion> Completions { get; }
    public int? Complete_Index { get; private set; }

    public Completion? Current => Complete_Index is { } i && i >= 0 && i < Completions.Count ? Completions[i] : null;

    public void GoToNext()
    {
        if (Completions.Count == 0) { Complete_Index = null; return; }
        Complete_Index = Complete_Index is null ? 0 : (Complete_Index.Value + 1) % Completions.Count;
    }

    public void GoToPrevious()
    {
        if (Completions.Count == 0) { Complete_Index = null; return; }
        Complete_Index = Complete_Index is null ? Completions.Count - 1 : (Complete_Index.Value - 1 + Completions.Count) % Completions.Count;
    }
}
