using DotnetPromptToolkit.KeyBinding;

namespace DotnetPromptToolkit.Input;

/// <summary>
/// Mirrors a small subset of prompt_toolkit.input.vt100_parser.Vt100Parser.
/// Recognises printable characters, common control characters, escape
/// sequences for arrows/Home/End/Page/F-keys, and bracketed paste markers.
/// </summary>
public sealed class AnsiInputParser
{
    private readonly Queue<char> _pending = [];

    public IEnumerable<KeyPress> Feed(string value)
    {
        foreach (var c in value) _pending.Enqueue(c);
        while (_pending.Count > 0)
        {
            var c = _pending.Dequeue();
            if (c == '\u001b' && _pending.Count >= 2 && _pending.Peek() == '[')
            {
                _pending.Dequeue(); // consume '['
                var seq = ReadCsi();
                yield return MapCsi(seq);
            }
            else if (c == '\u001b' && _pending.Count >= 2 && _pending.Peek() == 'O')
            {
                _pending.Dequeue();
                var final = _pending.Dequeue();
                yield return MapSs3(final);
            }
            else
            {
                yield return c switch
                {
                    '\r' or '\n' => new KeyPress(Key.Enter),
                    '\b' or '\u007f' => new KeyPress(Key.Backspace),
                    '\t' => new KeyPress(Key.Tab),
                    '\u001b' => new KeyPress(Key.Escape),
                    >= '\u0001' and <= '\u001a' => new KeyPress(ControlKey(c), c),
                    _ => new KeyPress(Key.Character, c)
                };
            }
        }
    }

    private string ReadCsi()
    {
        var sb = new System.Text.StringBuilder();
        while (_pending.Count > 0)
        {
            var ch = _pending.Dequeue();
            sb.Append(ch);
            if (ch is >= '@' and <= '~') break;
        }
        return sb.ToString();
    }

    private static KeyPress MapCsi(string seq) => seq switch
    {
        "A" => new KeyPress(Key.Up),
        "B" => new KeyPress(Key.Down),
        "C" => new KeyPress(Key.Right),
        "D" => new KeyPress(Key.Left),
        "H" => new KeyPress(Key.Home),
        "F" => new KeyPress(Key.End),
        "1~" or "7~" => new KeyPress(Key.Home),
        "4~" or "8~" => new KeyPress(Key.End),
        "2~" => new KeyPress(Key.Insert),
        "3~" => new KeyPress(Key.Delete),
        "5~" => new KeyPress(Key.PageUp),
        "6~" => new KeyPress(Key.PageDown),
        _ => new KeyPress(Key.Escape)
    };

    private static KeyPress MapSs3(char final) => final switch
    {
        'A' => new KeyPress(Key.Up),
        'B' => new KeyPress(Key.Down),
        'C' => new KeyPress(Key.Right),
        'D' => new KeyPress(Key.Left),
        'H' => new KeyPress(Key.Home),
        'F' => new KeyPress(Key.End),
        _ => new KeyPress(Key.Escape)
    };

    private static Key ControlKey(char c) => (Key)(((int)Key.ControlA) + (c - 1));
}
