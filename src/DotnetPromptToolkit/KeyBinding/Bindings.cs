using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;

namespace DotnetPromptToolkit.KeyBinding;

/// <summary>
/// Default Emacs-style bindings, mirroring prompt_toolkit.key_binding.bindings.emacs.
/// Implements the most common edit operations (Ctrl-A, Ctrl-E, Ctrl-K, etc.).
/// </summary>
public static class EmacsBindings
{
    public static KeyBindings Create()
    {
        return new KeyBindings()
            .Add(Key.ControlA, (b, _) => b.CursorLeft(b.CursorPosition))
            .Add(Key.ControlE, (b, _) => b.CursorRight(b.Text.Length - b.CursorPosition))
            .Add(Key.ControlK, (b, _) =>
            {
                var doc = b.Document;
                var lineEnd = doc.CursorPosition + doc.CurrentLineAfterCursor.Length;
                var deleteCount = lineEnd - doc.CursorPosition;
                if (deleteCount > 0)
                {
                    b.CursorRight(deleteCount);
                    b.DeleteBeforeCursor(deleteCount);
                }
            })
            .Add(Key.ControlU, (b, _) =>
            {
                var lineStart = b.CursorPosition - b.Document.CurrentLineBeforeCursor.Length;
                var count = b.CursorPosition - lineStart;
                if (count > 0) b.DeleteBeforeCursor(count);
            });
    }
}

/// <summary>
/// Minimal Vi-style bindings, mirroring prompt_toolkit.key_binding.bindings.vi.
/// Only navigation primitives are implemented to keep the surface small.
/// </summary>
public static class ViBindings
{
    public static KeyBindings Create()
    {
        return new KeyBindings()
            .Add(Key.Character, (b, k) =>
            {
                if (k.Character == 'h') b.CursorLeft();
                else if (k.Character == 'l') b.CursorRight();
                else b.InsertText(k.Character?.ToString() ?? string.Empty);
            }, character: null);
    }
}
