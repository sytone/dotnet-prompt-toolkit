namespace DotnetPromptToolkit.Selection;

/// <summary>Mirrors prompt_toolkit.selection.SelectionType.</summary>
public enum SelectionType
{
    Characters,
    Lines,
    Block
}

/// <summary>Mirrors prompt_toolkit.selection.SelectionState.</summary>
public sealed class SelectionState
{
    public SelectionState(int originalCursorPosition = 0, SelectionType type = SelectionType.Characters)
    {
        OriginalCursorPosition = originalCursorPosition;
        Type = type;
    }

    public int OriginalCursorPosition { get; set; }
    public SelectionType Type { get; set; }
    public bool ShiftMode { get; set; }

    public SelectionState Clone() => new(OriginalCursorPosition, Type) { ShiftMode = ShiftMode };

    public (int Start, int End) GetRange(int cursor)
    {
        var lo = Math.Min(OriginalCursorPosition, cursor);
        var hi = Math.Max(OriginalCursorPosition, cursor) + 1; // python end-inclusive
        return (lo, hi);
    }
}

public sealed record PasteMode(string Name)
{
    public static readonly PasteMode Emacs = new("emacs");
    public static readonly PasteMode ViAfter = new("vi_after");
    public static readonly PasteMode ViBefore = new("vi_before");
}
