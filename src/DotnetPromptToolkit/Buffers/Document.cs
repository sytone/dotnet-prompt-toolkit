namespace DotnetPromptToolkit.Buffers;

public sealed record CursorPosition(int Row, int Column);

public sealed class Document
{
    public Document(string text = "", int? cursorPosition = null)
    {
        Text = text ?? string.Empty;
        CursorPosition = Math.Clamp(cursorPosition ?? Text.Length, 0, Text.Length);
    }

    public string Text { get; }
    public int CursorPosition { get; }
    public string TextBeforeCursor => Text[..CursorPosition];
    public string TextAfterCursor => Text[CursorPosition..];
    public IReadOnlyList<string> Lines => Text.Replace("\r\n", "\n").Split('\n');
    public string CurrentLine => Lines[TranslateIndexToPosition(CursorPosition).Row];
    public string CurrentLineBeforeCursor => CurrentLine[..TranslateIndexToPosition(CursorPosition).Column];
    public string CurrentLineAfterCursor => CurrentLine[TranslateIndexToPosition(CursorPosition).Column..];
    public bool IsCursorAtEnd => CursorPosition == Text.Length;

    public CursorPosition TranslateIndexToPosition(int index)
    {
        index = Math.Clamp(index, 0, Text.Length);
        var row = 0;
        var column = 0;
        for (var i = 0; i < index; i++)
        {
            if (Text[i] == '\n') { row++; column = 0; }
            else if (Text[i] != '\r') { column++; }
        }
        return new CursorPosition(row, column);
    }

    public int TranslatePositionToIndex(int row, int column)
    {
        row = Math.Max(0, row);
        column = Math.Max(0, column);
        var currentRow = 0;
        var currentColumn = 0;
        for (var i = 0; i < Text.Length; i++)
        {
            if (currentRow == row && currentColumn == column) return i;
            if (Text[i] == '\n') { currentRow++; currentColumn = 0; }
            else if (Text[i] != '\r') { currentColumn++; }
        }
        return Text.Length;
    }

    public string GetWordBeforeCursor(Func<char, bool>? predicate = null)
    {
        predicate ??= static c => char.IsLetterOrDigit(c) || c == '_';
        var start = CursorPosition;
        while (start > 0 && predicate(Text[start - 1])) start--;
        return Text[start..CursorPosition];
    }

    public Document InsertBeforeCursor(string value) => new(TextBeforeCursor + (value ?? string.Empty) + TextAfterCursor, CursorPosition + (value?.Length ?? 0));

    public Document DeleteBeforeCursor(int count = 1)
    {
        count = Math.Clamp(count, 0, CursorPosition);
        return new Document(Text[..(CursorPosition - count)] + TextAfterCursor, CursorPosition - count);
    }

    public Document MoveCursor(int offset) => new(Text, CursorPosition + offset);
}
