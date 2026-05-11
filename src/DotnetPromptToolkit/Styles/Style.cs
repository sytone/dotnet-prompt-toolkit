namespace DotnetPromptToolkit.Styles;

public sealed record StyleRule(string Selector, string Value);

public sealed class Style
{
    private readonly Dictionary<string, string> _rules;
    public Style(IEnumerable<StyleRule> rules) => _rules = rules.ToDictionary(r => r.Selector, r => r.Value, StringComparer.Ordinal);
    public static Style Empty { get; } = new([]);
    public static Style FromDictionary(IReadOnlyDictionary<string, string> values) => new(values.Select(kvp => new StyleRule(kvp.Key, kvp.Value)));
    public string GetStyleForToken(string selector) => _rules.TryGetValue(selector, out var value) ? value : string.Empty;
}
