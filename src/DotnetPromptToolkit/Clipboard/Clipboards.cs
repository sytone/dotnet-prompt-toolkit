namespace DotnetPromptToolkit.Clipboard;

/// <summary>Mirrors prompt_toolkit.clipboard.ClipboardData.</summary>
public sealed record ClipboardData(string Text, ClipboardDataType Type = ClipboardDataType.Characters);

public enum ClipboardDataType
{
    Characters,
    Lines,
    Block
}

/// <summary>Mirrors prompt_toolkit.clipboard.Clipboard with rich data support.</summary>
public interface IRichClipboard
{
    void SetData(ClipboardData data);
    ClipboardData GetData();
    void Rotate();
}

/// <summary>Implements a kill-ring style clipboard. Mirrors InMemoryClipboard.</summary>
public sealed class KillRingClipboard(int maxSize = 60) : IRichClipboard
{
    private readonly LinkedList<ClipboardData> _ring = new();

    public void SetData(ClipboardData data)
    {
        _ring.AddFirst(data);
        while (_ring.Count > maxSize) _ring.RemoveLast();
    }

    public ClipboardData GetData() => _ring.First?.Value ?? new ClipboardData(string.Empty);

    public void Rotate()
    {
        if (_ring.Count <= 1) return;
        var first = _ring.First!;
        _ring.RemoveFirst();
        _ring.AddLast(first.Value);
    }

    public IReadOnlyCollection<ClipboardData> History => _ring;
}

/// <summary>Mirrors prompt_toolkit.clipboard.PyperclipClipboard via System OSC52 fallback.</summary>
public sealed class DynamicClipboard(Func<IClipboard> resolver) : IClipboard
{
    public string Text { get => resolver().Text; set => resolver().Text = value; }
}
