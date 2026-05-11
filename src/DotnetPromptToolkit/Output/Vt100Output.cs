using System.Text;

namespace DotnetPromptToolkit.Output;

/// <summary>
/// Mirrors prompt_toolkit.output.vt100.Vt100_Output. Buffers ANSI sequences and
/// writes them on flush. Color depth controls how foreground colors are
/// rendered.
/// </summary>
public sealed class Vt100Output(TextWriter writer, ColorDepth colorDepth = ColorDepth.Depth4Bit) : ITerminalOutput
{
    private readonly TextWriter _writer = writer;
    private readonly StringBuilder _buffer = new();
    private readonly ColorDepth _colorDepth = colorDepth;

    public ColorDepth ColorDepth => _colorDepth;

    public void Write(string value) => _buffer.Append(value);
    public void WriteLine(string value) => _buffer.AppendLine(value);
    public void WriteRaw(string sequence) => _buffer.Append(sequence);

    public void CursorUp(int amount = 1) => _buffer.Append($"\u001b[{amount}A");
    public void CursorDown(int amount = 1) => _buffer.Append($"\u001b[{amount}B");
    public void CursorForward(int amount = 1) => _buffer.Append($"\u001b[{amount}C");
    public void CursorBackward(int amount = 1) => _buffer.Append($"\u001b[{amount}D");
    public void CursorGoto(int row, int column) => _buffer.Append($"\u001b[{row + 1};{column + 1}H");
    public void EnableAlternateScreen() => _buffer.Append("\u001b[?1049h");
    public void DisableAlternateScreen() => _buffer.Append("\u001b[?1049l");
    public void HideCursor() => _buffer.Append("\u001b[?25l");
    public void ShowCursor() => _buffer.Append("\u001b[?25h");
    public void Reset() => _buffer.Append(Ansi.Reset);
    public void EraseDown() => _buffer.Append("\u001b[J");
    public void EraseEndOfLine() => _buffer.Append("\u001b[K");
    public void EnableBracketedPaste() => _buffer.Append("\u001b[?2004h");
    public void DisableBracketedPaste() => _buffer.Append("\u001b[?2004l");

    public void SetForeground(int colorIndex)
    {
        switch (_colorDepth)
        {
            case ColorDepth.Depth1Bit:
                break;
            case ColorDepth.Depth4Bit:
                _buffer.Append($"\u001b[{30 + (colorIndex & 7)}m");
                break;
            case ColorDepth.Depth8Bit:
                _buffer.Append($"\u001b[38;5;{colorIndex & 0xff}m");
                break;
            case ColorDepth.Depth24Bit:
                var r = (colorIndex >> 16) & 0xff;
                var g = (colorIndex >> 8) & 0xff;
                var b = colorIndex & 0xff;
                _buffer.Append($"\u001b[38;2;{r};{g};{b}m");
                break;
        }
    }

    public void Flush()
    {
        if (_buffer.Length == 0) return;
        _writer.Write(_buffer.ToString());
        _writer.Flush();
        _buffer.Clear();
    }

    /// <summary>Returns and clears the buffered output - useful for tests.</summary>
    public string DrainBuffer()
    {
        var value = _buffer.ToString();
        _buffer.Clear();
        return value;
    }
}

/// <summary>Mirrors prompt_toolkit.output.plain_text.PlainTextOutput - swallows formatting.</summary>
public sealed class PlainTextOutput(TextWriter writer) : ITerminalOutput
{
    public void Write(string value) => writer.Write(value);
    public void WriteLine(string value) => writer.WriteLine(value);
}

/// <summary>Mirrors prompt_toolkit.output.DummyOutput.</summary>
public sealed class DummyOutput : ITerminalOutput
{
    public void Write(string value) { }
    public void WriteLine(string value) { }
}
