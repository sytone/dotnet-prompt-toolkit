using DotnetPromptToolkit.FormattedText;
using DotnetPromptToolkit.Layout;

namespace DotnetPromptToolkit.Widgets;

/// <summary>Mirrors prompt_toolkit.widgets.base.Frame.</summary>
public sealed class Frame(IRenderable body, string? title = null) : IRenderable
{
    public IRenderable Body { get; } = body;
    public string? Title { get; } = title;

    public FormattedText.FormattedText Render()
    {
        var content = Body.Render().ToPlainText();
        var lines = content.Replace("\r\n", "\n").Split('\n');
        var width = Math.Max(Title?.Length ?? 0, lines.Max(l => l.Length)) + 2;
        var top = "┌" + (Title is null ? new string('─', width) : ("┤ " + Title + " ├").PadRight(width, '─')) + "┐";
        var bottom = "└" + new string('─', width) + "┘";
        var inside = string.Join('\n', lines.Select(l => "│ " + l.PadRight(width - 2) + " │"));
        return FormattedText.FormattedText.FromPlainText(top + "\n" + inside + "\n" + bottom);
    }
}

/// <summary>Mirrors prompt_toolkit.widgets.base.Box (just adds padding).</summary>
public sealed class Box(IRenderable body, int padding = 1) : IRenderable
{
    public IRenderable Body { get; } = body;
    public int Padding { get; } = padding;

    public FormattedText.FormattedText Render()
    {
        var content = Body.Render().ToPlainText();
        var pad = new string(' ', Padding);
        var blank = string.Concat(Enumerable.Repeat(Environment.NewLine, Padding));
        var lines = content.Replace("\r\n", "\n").Split('\n');
        var inside = string.Join(Environment.NewLine, lines.Select(l => pad + l + pad));
        return FormattedText.FormattedText.FromPlainText(blank + inside + blank);
    }
}

/// <summary>Mirrors prompt_toolkit.widgets.base.Label.</summary>
public sealed class TextLabel(string text, string style = "") : IRenderable
{
    public string Text { get; set; } = text;
    public string Style { get; set; } = style;
    public FormattedText.FormattedText Render() => new FormattedText.FormattedText().Append(Text, Style);
}

/// <summary>Mirrors prompt_toolkit.widgets.base.Button.</summary>
public sealed class Button(string text, Action? handler = null, int width = 12) : IRenderable
{
    public string Text { get; set; } = text;
    public Action? Handler { get; } = handler;
    public int Width { get; set; } = width;
    public bool Focused { get; set; }

    public void Click() => Handler?.Invoke();

    public FormattedText.FormattedText Render()
    {
        var label = $"< {Text} >".PadRight(Width);
        return new FormattedText.FormattedText().Append(label, Focused ? "class:button.focused" : "class:button");
    }
}

/// <summary>Mirrors prompt_toolkit.widgets.base.Checkbox.</summary>
public sealed class Checkbox(string text, bool isChecked = false) : IRenderable
{
    public string Text { get; set; } = text;
    public bool IsChecked { get; set; } = isChecked;
    public void Toggle() => IsChecked = !IsChecked;
    public FormattedText.FormattedText Render() => new FormattedText.FormattedText().Append((IsChecked ? "[x] " : "[ ] ") + Text, "class:checkbox");
}

/// <summary>Mirrors prompt_toolkit.widgets.base.RadioList.</summary>
public sealed class RadioList<T>(IEnumerable<(T Value, string Label)> options) : IRenderable
{
    public List<(T Value, string Label)> Options { get; } = options.ToList();
    public int CurrentIndex { get; set; }
    public T? Current => Options.Count == 0 ? default : Options[CurrentIndex].Value;

    public void Next() { if (Options.Count > 0) CurrentIndex = (CurrentIndex + 1) % Options.Count; }
    public void Previous() { if (Options.Count > 0) CurrentIndex = (CurrentIndex - 1 + Options.Count) % Options.Count; }

    public FormattedText.FormattedText Render()
    {
        var formatted = new FormattedText.FormattedText();
        for (var i = 0; i < Options.Count; i++)
        {
            var marker = i == CurrentIndex ? "(*)" : "( )";
            formatted.Append(marker + " " + Options[i].Label + "\n", i == CurrentIndex ? "class:radio-selected" : "class:radio");
        }
        return formatted;
    }
}

/// <summary>Mirrors prompt_toolkit.widgets.menus.MenuItem.</summary>
public sealed class MenuItem(string text, Action? handler = null, IEnumerable<MenuItem>? children = null)
{
    public string Text { get; } = text;
    public Action? Handler { get; } = handler;
    public List<MenuItem> Children { get; } = children?.ToList() ?? new();
}

/// <summary>Mirrors prompt_toolkit.widgets.menus.MenuContainer.</summary>
public sealed class MenuContainer(IRenderable body, IEnumerable<MenuItem> menuItems) : IRenderable
{
    public IRenderable Body { get; } = body;
    public List<MenuItem> MenuItems { get; } = menuItems.ToList();

    public FormattedText.FormattedText Render()
    {
        var menuLine = string.Join(" | ", MenuItems.Select(m => m.Text));
        var formatted = new FormattedText.FormattedText().Append(menuLine + Environment.NewLine, "class:menu");
        formatted.Append(Body.Render().ToPlainText());
        return formatted;
    }
}

/// <summary>Mirrors prompt_toolkit.widgets.toolbars.FormattedTextToolbar.</summary>
public sealed class FormattedTextToolbar(string text, string style = "class:bottom-toolbar") : IRenderable
{
    public string Text { get; set; } = text;
    public string Style { get; set; } = style;
    public FormattedText.FormattedText Render() => new FormattedText.FormattedText().Append(Text, Style);
}

/// <summary>Mirrors prompt_toolkit.widgets.dialogs.Dialog.</summary>
public sealed class Dialog(string title, IRenderable body, IEnumerable<Button>? buttons = null) : IRenderable
{
    public string Title { get; } = title;
    public IRenderable Body { get; } = body;
    public List<Button> Buttons { get; } = buttons?.ToList() ?? new();

    public FormattedText.FormattedText Render()
    {
        var content = Body.Render().ToPlainText();
        if (Buttons.Count > 0)
        {
            content += Environment.NewLine + string.Join("  ", Buttons.Select(b => b.Render().ToPlainText().TrimEnd()));
        }
        return new Frame(new TextLabel(content), Title).Render();
    }
}
