using DotnetPromptToolkit.FormattedText;

namespace DotnetPromptToolkit.Rendering;

public sealed class Screen
{
    private readonly Dictionary<(int Row, int Column), char> _cells = [];
    public void Write(int row, int column, string text)
    {
        for (var i = 0; i < text.Length; i++) _cells[(row, column + i)] = text[i];
    }
    public string GetLine(int row)
    {
        var cells = _cells.Where(kvp => kvp.Key.Row == row).OrderBy(kvp => kvp.Key.Column).ToArray();
        if (cells.Length == 0) return string.Empty;
        var max = cells[^1].Key.Column;
        var chars = Enumerable.Repeat(' ', max + 1).ToArray();
        foreach (var cell in cells) chars[cell.Key.Column] = cell.Value;
        return new string(chars).TrimEnd();
    }
}

public sealed record RenderOperation(int Row, int Column, string Text);

public sealed class Renderer
{
    private string _last = string.Empty;
    public IReadOnlyList<RenderOperation> Render(FormattedText.FormattedText text)
    {
        var current = text.ToPlainText();
        if (current == _last) return [];
        _last = current;
        return current.Replace("\r\n", "\n").Split('\n').Select((line, row) => new RenderOperation(row, 0, line)).ToArray();
    }
}
