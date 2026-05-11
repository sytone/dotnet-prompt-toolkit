using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using DotnetPromptToolkit.FormattedText;

namespace DotnetPromptToolkit.Layout;

public interface IRenderable
{
    FormattedText.FormattedText Render();
}

public sealed class Label(string text, string style = "") : IRenderable
{
    public string Text { get; set; } = text;
    public string Style { get; set; } = style;
    public FormattedText.FormattedText Render() => new FormattedText.FormattedText().Append(Text, Style);
}

public sealed class TextArea(PromptBuffer? buffer = null, string prompt = "") : IRenderable
{
    public PromptBuffer Buffer { get; } = buffer ?? new PromptBuffer();
    public string Prompt { get; set; } = prompt;
    public FormattedText.FormattedText Render() => FormattedText.FormattedText.FromPlainText(Prompt + Buffer.Text);
}

public abstract class Container(IReadOnlyList<IRenderable> children) : IRenderable
{
    public IReadOnlyList<IRenderable> Children { get; } = children;
    public abstract FormattedText.FormattedText Render();
}

public sealed class HSplit(params IRenderable[] children) : Container(children)
{
    public override FormattedText.FormattedText Render() => FormattedText.FormattedText.FromPlainText(string.Join(Environment.NewLine, Children.Select(c => c.Render().ToPlainText())));
}

public sealed class VSplit(params IRenderable[] children) : Container(children)
{
    public override FormattedText.FormattedText Render() => FormattedText.FormattedText.FromPlainText(string.Concat(Children.Select(c => c.Render().ToPlainText())));
}
