using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using DotnetPromptToolkit.FormattedText;

namespace DotnetPromptToolkit.Layout;

/// <summary>Mirrors prompt_toolkit.layout.controls.UIControl.</summary>
public interface IUIControl : IRenderable { }

/// <summary>Mirrors prompt_toolkit.layout.controls.FormattedTextControl.</summary>
public sealed class FormattedTextControl(IEnumerable<FormattedTextFragment>? fragments = null) : IUIControl
{
    public List<FormattedTextFragment> Fragments { get; } = fragments?.ToList() ?? new List<FormattedTextFragment>();
    public FormattedText.FormattedText Render() => new(Fragments);
}

/// <summary>Mirrors prompt_toolkit.layout.controls.BufferControl.</summary>
public sealed class BufferControl(PromptBuffer buffer) : IUIControl
{
    public PromptBuffer Buffer { get; } = buffer;
    public FormattedText.FormattedText Render() => FormattedText.FormattedText.FromPlainText(Buffer.Text);
}

/// <summary>Mirrors prompt_toolkit.layout.containers.Window.</summary>
public sealed class Window(IUIControl control, Dimension? width = null, Dimension? height = null) : IRenderable
{
    public IUIControl Control { get; } = control;
    public Dimension Width { get; } = width ?? new Dimension();
    public Dimension Height { get; } = height ?? new Dimension();
    public FormattedText.FormattedText Render() => Control.Render();
}

/// <summary>Mirrors prompt_toolkit.layout.containers.ScrollOffsets.</summary>
public sealed record ScrollOffsets(int Top = 0, int Bottom = 0, int Left = 0, int Right = 0);

/// <summary>Mirrors prompt_toolkit.layout.containers.Float.</summary>
public sealed record Float(IRenderable Content, int Top = 0, int Left = 0, int Width = 0, int Height = 0, bool Transparent = false);

/// <summary>Mirrors prompt_toolkit.layout.containers.FloatContainer.</summary>
public sealed class FloatContainer(IRenderable content, IEnumerable<Float>? floats = null) : IRenderable
{
    public IRenderable Content { get; } = content;
    public List<Float> Floats { get; } = floats?.ToList() ?? new List<Float>();

    public FormattedText.FormattedText Render()
    {
        var output = Content.Render();
        foreach (var floating in Floats)
        {
            output.Append(Environment.NewLine, "");
            output.Append(floating.Content.Render().ToPlainText(), "");
        }
        return output;
    }
}
