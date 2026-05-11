using DotnetPromptToolkit.FormattedText;
using DotnetPromptToolkit.MouseEvents;

namespace DotnetPromptToolkit.Layout;

/// <summary>Mirrors prompt_toolkit.layout.margins.Margin.</summary>
public interface IMargin
{
    int GetWidth();
    FormattedText.FormattedText CreateMargin(int height);
}

/// <summary>Mirrors prompt_toolkit.layout.margins.NumberedMargin.</summary>
public sealed class NumberedMargin(int firstLine = 1, int padding = 1) : IMargin
{
    public int GetWidth() => 4 + padding;
    public FormattedText.FormattedText CreateMargin(int height)
    {
        var formatted = new FormattedText.FormattedText();
        for (var i = 0; i < height; i++)
        {
            formatted.Append((firstLine + i).ToString().PadLeft(4) + new string(' ', padding) + "\n", "class:line-number");
        }
        return formatted;
    }
}

/// <summary>Mirrors prompt_toolkit.layout.margins.ScrollbarMargin.</summary>
public sealed class ScrollbarMargin(int contentHeight, int viewHeight, int viewTop) : IMargin
{
    public int GetWidth() => 1;
    public FormattedText.FormattedText CreateMargin(int height)
    {
        var formatted = new FormattedText.FormattedText();
        if (contentHeight <= 0 || viewHeight <= 0)
        {
            return formatted.Append(new string('|', height));
        }
        var thumbSize = Math.Max(1, viewHeight * height / contentHeight);
        var thumbStart = Math.Min(height - thumbSize, viewTop * height / contentHeight);
        for (var i = 0; i < height; i++)
        {
            var inThumb = i >= thumbStart && i < thumbStart + thumbSize;
            formatted.Append(inThumb ? "█" : "│");
        }
        return formatted;
    }
}

/// <summary>Mirrors prompt_toolkit.layout.processors.Processor.</summary>
public interface IProcessor
{
    FormattedText.FormattedText Process(FormattedText.FormattedText input);
}

/// <summary>Highlights matches of a search query, mirroring HighlightSearchProcessor.</summary>
public sealed class HighlightSearchProcessor(string searchText, string style = "class:search") : IProcessor
{
    public FormattedText.FormattedText Process(FormattedText.FormattedText input)
    {
        if (string.IsNullOrEmpty(searchText)) return input;
        var output = new FormattedText.FormattedText();
        foreach (var fragment in input.Fragments)
        {
            var text = fragment.Text;
            var idx = 0;
            while (idx < text.Length)
            {
                var match = text.IndexOf(searchText, idx, StringComparison.Ordinal);
                if (match < 0) { output.Append(text[idx..], fragment.Style); break; }
                if (match > idx) output.Append(text[idx..match], fragment.Style);
                output.Append(text.Substring(match, searchText.Length), style);
                idx = match + searchText.Length;
            }
        }
        return output;
    }
}

/// <summary>Mirrors prompt_toolkit.layout.processors.PasswordProcessor.</summary>
public sealed class PasswordProcessor(char mask = '*') : IProcessor
{
    public FormattedText.FormattedText Process(FormattedText.FormattedText input)
    {
        var output = new FormattedText.FormattedText();
        foreach (var fragment in input.Fragments)
        {
            output.Append(new string(mask, fragment.Text.Length), fragment.Style);
        }
        return output;
    }
}

/// <summary>Mirrors prompt_toolkit.layout.menus.CompletionsMenu rendering.</summary>
public sealed class CompletionsMenu(IEnumerable<string> completions, int? selectedIndex = null) : IRenderable
{
    public IReadOnlyList<string> Completions { get; } = completions.ToArray();
    public int? SelectedIndex { get; } = selectedIndex;

    public FormattedText.FormattedText Render()
    {
        var formatted = new FormattedText.FormattedText();
        for (var i = 0; i < Completions.Count; i++)
        {
            var style = i == SelectedIndex ? "class:completion-menu.completion.current" : "class:completion-menu.completion";
            formatted.Append(Completions[i] + "\n", style);
        }
        return formatted;
    }
}

/// <summary>Mirrors prompt_toolkit.layout.mouse_handlers.MouseHandler.</summary>
public delegate void MouseHandler(MouseEvent mouseEvent);
