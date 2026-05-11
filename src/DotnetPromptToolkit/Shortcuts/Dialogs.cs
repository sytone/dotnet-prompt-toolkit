using DotnetPromptToolkit.FormattedText;
using DotnetPromptToolkit.Layout;
using DotnetPromptToolkit.Widgets;

namespace DotnetPromptToolkit.Shortcuts;

/// <summary>
/// Static dialog renderers. Mirrors prompt_toolkit.shortcuts.dialogs by
/// producing renderable Dialog widgets and exposing simple synchronous helpers
/// for non-interactive scenarios.
/// </summary>
public static class Dialogs
{
    public static Dialog MessageDialog(string title, string text) =>
        new(title, new TextLabel(text), new[] { new Button("OK") });

    public static Dialog YesNoDialog(string title, string text) =>
        new(title, new TextLabel(text), new[] { new Button("Yes"), new Button("No") });

    public static Dialog InputDialog(string title, string text, string defaultValue = "") =>
        new(title, new TextLabel(text + Environment.NewLine + defaultValue), new[] { new Button("OK"), new Button("Cancel") });

    public static Dialog ButtonDialog(string title, string text, IEnumerable<(string Label, Action Handler)> buttons) =>
        new(title, new TextLabel(text), buttons.Select(b => new Button(b.Label, b.Handler)));

    public static Dialog ChoiceDialog<T>(string title, string text, IEnumerable<(T Value, string Label)> options)
    {
        var radio = new RadioList<T>(options);
        var body = new TextLabel(text + Environment.NewLine + radio.Render().ToPlainText());
        return new Dialog(title, body, new[] { new Button("OK"), new Button("Cancel") });
    }
}

/// <summary>
/// Mirrors prompt_toolkit.shortcuts.progress_bar.ProgressBar. Renders a simple
/// textual progress bar.
/// </summary>
public sealed class ProgressBar
{
    public ProgressBar(int total = 100, int width = 40)
    {
        if (total <= 0) throw new ArgumentOutOfRangeException(nameof(total));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        Total = total;
        Width = width;
    }

    public int Total { get; }
    public int Width { get; }
    public int Current { get; private set; }

    public void Advance(int amount = 1) => Current = Math.Min(Total, Current + amount);
    public void Reset() => Current = 0;

    public string Render()
    {
        var ratio = (double)Current / Total;
        var filled = (int)Math.Round(ratio * Width);
        var bar = new string('█', filled) + new string('░', Width - filled);
        var percent = (int)Math.Round(ratio * 100);
        return $"|{bar}| {Current,4}/{Total} ({percent}%)";
    }
}
