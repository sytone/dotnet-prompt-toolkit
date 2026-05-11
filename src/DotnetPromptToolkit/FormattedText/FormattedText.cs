namespace DotnetPromptToolkit.FormattedText;

public sealed record FormattedTextFragment(string Style, string Text);

public sealed class FormattedText
{
    private readonly List<FormattedTextFragment> _fragments = [];
    public FormattedText() { }
    public FormattedText(IEnumerable<FormattedTextFragment> fragments) => _fragments.AddRange(fragments);
    public IReadOnlyList<FormattedTextFragment> Fragments => _fragments;
    public static FormattedText FromPlainText(string text) => new([new FormattedTextFragment(string.Empty, text)]);
    public FormattedText Append(string text, string style = "") { _fragments.Add(new FormattedTextFragment(style, text)); return this; }
    public string ToPlainText() => string.Concat(_fragments.Select(f => f.Text));
}
