namespace DotnetPromptToolkit.History;

/// <summary>File-backed history, one entry per stored line. Mirrors prompt_toolkit.history.FileHistory.</summary>
public sealed class FileHistory(string path) : IHistory
{
    private readonly string _path = path;
    private readonly List<string> _items = LoadInitial(path);

    public void Append(string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        _items.Add(value);
        try
        {
            using var writer = new StreamWriter(_path, append: true);
            writer.WriteLine($"# {DateTimeOffset.UtcNow:O}");
            foreach (var line in value.Replace("\r\n", "\n").Split('\n'))
            {
                writer.WriteLine($"+{line}");
            }
            writer.WriteLine();
        }
        catch (IOException) { /* best-effort */ }
        catch (UnauthorizedAccessException) { /* best-effort */ }
    }

    public IReadOnlyList<string> GetStrings() => _items;

    private static List<string> LoadInitial(string path)
    {
        var items = new List<string>();
        if (!File.Exists(path)) return items;
        try
        {
            string? current = null;
            foreach (var rawLine in File.ReadAllLines(path))
            {
                if (rawLine.Length == 0)
                {
                    if (current is not null) items.Add(current);
                    current = null;
                }
                else if (rawLine.StartsWith("+", StringComparison.Ordinal))
                {
                    current = current is null ? rawLine[1..] : current + "\n" + rawLine[1..];
                }
            }
            if (current is not null) items.Add(current);
        }
        catch (IOException) { }
        return items;
    }
}

/// <summary>
/// Mirrors prompt_toolkit.history.ThreadedHistory. Wraps an inner history and
/// loads strings via a background task.
/// </summary>
public sealed class ThreadedHistory(IHistory inner) : IHistory
{
    private readonly IHistory _inner = inner;
    private Task<IReadOnlyList<string>>? _loader;

    public void Append(string value) => _inner.Append(value);

    public IReadOnlyList<string> GetStrings()
    {
        _loader ??= Task.Run(() => (IReadOnlyList<string>)_inner.GetStrings().ToArray());
        return _loader.GetAwaiter().GetResult();
    }
}
