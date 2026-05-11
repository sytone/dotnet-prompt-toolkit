using DotnetPromptToolkit.KeyBinding;

namespace DotnetPromptToolkit.Input;

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
                _pending.Dequeue();
                yield return (_pending.Dequeue()) switch
                {
                    'A' => new KeyPress(Key.Up),
                    'B' => new KeyPress(Key.Down),
                    'C' => new KeyPress(Key.Right),
                    'D' => new KeyPress(Key.Left),
                    _ => new KeyPress(Key.Escape)
                };
            }
            else
            {
                yield return c switch
                {
                    '\r' or '\n' => new KeyPress(Key.Enter),
                    '\b' or '\u007f' => new KeyPress(Key.Backspace),
                    '\u0003' => new KeyPress(Key.ControlC),
                    '\u0004' => new KeyPress(Key.ControlD),
                    '\u001b' => new KeyPress(Key.Escape),
                    _ => new KeyPress(Key.Character, c)
                };
            }
        }
    }
}
