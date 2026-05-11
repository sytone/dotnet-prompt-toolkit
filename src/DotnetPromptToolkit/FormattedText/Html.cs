using System.Text.RegularExpressions;

namespace DotnetPromptToolkit.FormattedText;

/// <summary>
/// Mirrors prompt_toolkit.formatted_text.HTML. Converts a small subset of HTML
/// into FormattedText fragments. Supported tags: &lt;b&gt;, &lt;i&gt;, &lt;u&gt;,
/// &lt;s&gt;, &lt;ansired&gt;, &lt;ansigreen&gt;, &lt;ansiblue&gt;, &lt;ansiyellow&gt;,
/// and &lt;style fg="..." bg="..."&gt;...&lt;/style&gt;.
/// </summary>
public static class HtmlFormattedText
{
    private static readonly Regex TagRegex = new(@"<(/?)([a-zA-Z][a-zA-Z0-9]*)([^>]*)>", RegexOptions.Compiled);
    private static readonly Regex AttrRegex = new(@"(\w+)\s*=\s*""([^""]*)""", RegexOptions.Compiled);

    public static FormattedText Parse(string html)
    {
        if (string.IsNullOrEmpty(html)) return new FormattedText();
        var fragments = new FormattedText();
        var styleStack = new Stack<string>();
        var i = 0;
        while (i < html.Length)
        {
            var match = TagRegex.Match(html, i);
            if (!match.Success)
            {
                fragments.Append(Decode(html[i..]), CurrentStyle(styleStack));
                break;
            }
            if (match.Index > i)
            {
                fragments.Append(Decode(html[i..match.Index]), CurrentStyle(styleStack));
            }
            var closing = match.Groups[1].Value == "/";
            var name = match.Groups[2].Value.ToLowerInvariant();
            var attributes = match.Groups[3].Value;
            if (closing)
            {
                if (styleStack.Count > 0) styleStack.Pop();
            }
            else
            {
                styleStack.Push(StyleForTag(name, attributes));
            }
            i = match.Index + match.Length;
        }
        return fragments;
    }

    private static string CurrentStyle(Stack<string> stack)
    {
        if (stack.Count == 0) return string.Empty;
        return string.Join(" ", stack.Reverse().Where(s => !string.IsNullOrEmpty(s)));
    }

    private static string StyleForTag(string name, string attributes)
    {
        return name switch
        {
            "style" => StyleAttribute(attributes),
            _ => $"class:{name}"
        };
    }

    private static string StyleAttribute(string attributes)
    {
        var parts = new List<string>();
        foreach (Match m in AttrRegex.Matches(attributes))
        {
            var key = m.Groups[1].Value.ToLowerInvariant();
            var value = m.Groups[2].Value;
            if (key == "fg") parts.Add($"fg:{value}");
            else if (key == "bg") parts.Add($"bg:{value}");
        }
        return string.Join(" ", parts);
    }

    private static string Decode(string text) => text
        .Replace("&lt;", "<")
        .Replace("&gt;", ">")
        .Replace("&amp;", "&")
        .Replace("&quot;", "\"");
}

/// <summary>Mirrors prompt_toolkit.formatted_text.ANSI - parses CSI SGR sequences.</summary>
public static class AnsiFormattedText
{
    public static FormattedText Parse(string ansi)
    {
        var formatted = new FormattedText();
        if (string.IsNullOrEmpty(ansi)) return formatted;
        var current = string.Empty;
        var i = 0;
        var buffer = new System.Text.StringBuilder();
        while (i < ansi.Length)
        {
            if (ansi[i] == '\u001b' && i + 1 < ansi.Length && ansi[i + 1] == '[')
            {
                if (buffer.Length > 0)
                {
                    formatted.Append(buffer.ToString(), current);
                    buffer.Clear();
                }
                var end = ansi.IndexOf('m', i);
                if (end < 0) break;
                var codes = ansi.Substring(i + 2, end - i - 2);
                current = SgrToStyle(codes, current);
                i = end + 1;
            }
            else
            {
                buffer.Append(ansi[i]);
                i++;
            }
        }
        if (buffer.Length > 0) formatted.Append(buffer.ToString(), current);
        return formatted;
    }

    private static string SgrToStyle(string codes, string current)
    {
        if (string.IsNullOrEmpty(codes) || codes == "0") return string.Empty;
        var attrs = new HashSet<string>(current.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        foreach (var raw in codes.Split(';'))
        {
            if (!int.TryParse(raw, out var code)) continue;
            switch (code)
            {
                case 0: attrs.Clear(); break;
                case 1: attrs.Add("bold"); break;
                case 3: attrs.Add("italic"); break;
                case 4: attrs.Add("underline"); break;
                case 7: attrs.Add("reverse"); break;
                case 9: attrs.Add("strike"); break;
                case >= 30 and <= 37: attrs.Add($"fg:ansi{NameForCode(code - 30)}"); break;
                case >= 40 and <= 47: attrs.Add($"bg:ansi{NameForCode(code - 40)}"); break;
            }
        }
        return string.Join(" ", attrs);
    }

    private static string NameForCode(int idx) => idx switch
    {
        0 => "black", 1 => "red", 2 => "green", 3 => "yellow",
        4 => "blue", 5 => "magenta", 6 => "cyan", 7 => "white",
        _ => "default"
    };
}

/// <summary>Mirrors prompt_toolkit.formatted_text.utils.fragment_list_to_text.</summary>
public static class FormattedTextUtils
{
    public static string FragmentListToText(IEnumerable<FormattedTextFragment> fragments) =>
        string.Concat(fragments.Select(f => f.Text));

    public static int FragmentListWidth(IEnumerable<FormattedTextFragment> fragments) =>
        FragmentListToText(fragments).Length;

    public static IReadOnlyList<IReadOnlyList<FormattedTextFragment>> SplitLines(IEnumerable<FormattedTextFragment> fragments)
    {
        var lines = new List<List<FormattedTextFragment>>();
        var current = new List<FormattedTextFragment>();
        foreach (var fragment in fragments)
        {
            var pieces = fragment.Text.Replace("\r\n", "\n").Split('\n');
            for (var i = 0; i < pieces.Length; i++)
            {
                if (i > 0)
                {
                    lines.Add(current);
                    current = new List<FormattedTextFragment>();
                }
                current.Add(new FormattedTextFragment(fragment.Style, pieces[i]));
            }
        }
        lines.Add(current);
        return lines;
    }
}
