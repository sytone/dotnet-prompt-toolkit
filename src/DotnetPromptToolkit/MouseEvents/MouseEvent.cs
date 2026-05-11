using DotnetPromptToolkit.DataStructures;

namespace DotnetPromptToolkit.MouseEvents;

public enum MouseEventType
{
    MouseDown,
    MouseUp,
    MouseMove,
    ScrollUp,
    ScrollDown
}

public enum MouseButton
{
    None,
    Left,
    Middle,
    Right
}

[Flags]
public enum MouseModifier
{
    None = 0,
    Shift = 1 << 0,
    Alt = 1 << 1,
    Control = 1 << 2
}

/// <summary>Mirrors prompt_toolkit.mouse_events.MouseEvent.</summary>
public sealed record MouseEvent(
    Point Position,
    MouseEventType EventType,
    MouseButton Button = MouseButton.None,
    MouseModifier Modifiers = MouseModifier.None);
