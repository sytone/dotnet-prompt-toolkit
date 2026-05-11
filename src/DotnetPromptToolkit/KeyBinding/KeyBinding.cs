using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;

namespace DotnetPromptToolkit.KeyBinding;

public enum Key
{
    Character,
    Enter,
    Backspace,
    Left,
    Right,
    Up,
    Down,
    Escape,
    ControlC,
    ControlD
}

public sealed record KeyPress(Key Key, char? Character = null);
public sealed record KeyBinding(Key Key, Action<PromptBuffer, KeyPress> Handler, char? Character = null);

public sealed class KeyBindings
{
    private readonly List<KeyBinding> _bindings = [];
    public IReadOnlyList<KeyBinding> Bindings => _bindings;
    public KeyBindings Add(Key key, Action<PromptBuffer, KeyPress> handler, char? character = null)
    {
        _bindings.Add(new KeyBinding(key, handler, character));
        return this;
    }
    public bool TryHandle(PromptBuffer buffer, KeyPress keyPress)
    {
        var binding = _bindings.LastOrDefault(b => b.Key == keyPress.Key && (b.Character is null || b.Character == keyPress.Character));
        if (binding is null) return false;
        binding.Handler(buffer, keyPress);
        return true;
    }
}

public sealed class KeyProcessor(KeyBindings keyBindings)
{
    public void Feed(PromptBuffer buffer, KeyPress keyPress)
    {
        if (keyBindings.TryHandle(buffer, keyPress)) return;
        switch (keyPress.Key)
        {
            case Key.Character when keyPress.Character is { } c: buffer.InsertText(c.ToString()); break;
            case Key.Backspace: buffer.DeleteBeforeCursor(); break;
            case Key.Left: buffer.CursorLeft(); break;
            case Key.Right: buffer.CursorRight(); break;
        }
    }
}
