namespace DotnetPromptToolkit.CursorShapes;

/// <summary>Mirrors prompt_toolkit.cursor_shapes.CursorShape.</summary>
public enum CursorShape
{
    Never,
    Block,
    Beam,
    Underline,
    BlinkingBlock,
    BlinkingBeam,
    BlinkingUnderline
}

public interface ICursorShapeConfig
{
    CursorShape GetCursorShape();
}

public sealed class StaticCursorShape(CursorShape shape) : ICursorShapeConfig
{
    public CursorShape Shape { get; } = shape;
    public CursorShape GetCursorShape() => Shape;
}

public sealed class DynamicCursorShapeConfig(Func<CursorShape> getter) : ICursorShapeConfig
{
    public CursorShape GetCursorShape() => getter();
}
