using System.Text.RegularExpressions;

namespace DotnetPromptToolkit.Contrib.RegularLanguages;

/// <summary>
/// Mirrors prompt_toolkit.contrib.regular_languages.compiler at a small scale.
/// Compiles a grammar string with named placeholders (e.g. "(?P&lt;name&gt;...)")
/// into a compiled regular expression that can match input prefixes and report
/// captured variable values.
/// </summary>
public sealed class CompiledGrammar
{
    private readonly Regex _exact;
    private readonly Regex _prefix;
    private readonly string _pattern;

    public CompiledGrammar(string pattern)
    {
        _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        _exact = new Regex("^(?:" + pattern + ")$");
        _prefix = new Regex("^(?:" + pattern + ")");
    }

    public string Pattern => _pattern;

    public Match? Match(string input)
    {
        var match = _exact.Match(input);
        return match.Success ? match : null;
    }

    public Match? MatchPrefix(string input)
    {
        var match = _prefix.Match(input);
        return match.Success ? match : null;
    }

    public IReadOnlyDictionary<string, string> Variables(string input)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var match = MatchPrefix(input);
        if (match is null) return result;
        foreach (var groupName in _prefix.GetGroupNames())
        {
            if (int.TryParse(groupName, out _)) continue;
            var group = match.Groups[groupName];
            if (group.Success) result[groupName] = group.Value;
        }
        return result;
    }
}

public static class GrammarCompiler
{
    public static CompiledGrammar Compile(string pattern) => new(pattern);
}
