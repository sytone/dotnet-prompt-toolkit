namespace DotnetPromptToolkit.Styles;

/// <summary>Mirrors prompt_toolkit.styles.defaults.default_ui_style.</summary>
public static class DefaultStyle
{
    public static IReadOnlyDictionary<string, string> Defaults { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["class:control-character"] = "ansiblue",
        ["class:cursor-line"] = "underline",
        ["class:cursor-column"] = "bg:#dddddd",
        ["class:scrollbar.background"] = "bg:#aaaaaa",
        ["class:scrollbar.button"] = "bg:#444444",
        ["class:scrollbar.arrow"] = "noinherit bold",
        ["class:line-number"] = "#888888",
        ["class:line-number.current"] = "bold #888888",
        ["class:tilde"] = "#8888ff",
        ["class:prompt"] = "",
        ["class:prompt.arg"] = "noinherit",
        ["class:prompt.arg.text"] = "",
        ["class:search"] = "noinherit reverse",
        ["class:search.text"] = "",
        ["class:system"] = "noinherit reverse",
        ["class:system.text"] = "",
        ["class:arg-toolbar"] = "noinherit reverse",
        ["class:arg-toolbar.text"] = "",
        ["class:bottom-toolbar"] = "reverse",
        ["class:bottom-toolbar.text"] = "",
        ["class:auto-suggestion"] = "#666666",
        ["class:completion-menu.completion"] = "bg:#008888 #ffffff",
        ["class:completion-menu.completion.current"] = "bg:#00aaaa #000000",
        ["class:completion-menu.meta.completion"] = "bg:#448844 #ffffff",
        ["class:completion-menu.meta.completion.current"] = "bg:#44aa44 #000000",
        ["class:completion-menu.multi-column-meta"] = "bg:#aaffaa #000000",
        ["class:scrollbar"] = "",
        ["class:menu.border"] = "",
        ["class:dialog"] = "bg:#4444ff",
        ["class:dialog.body"] = "bg:#cccccc",
        ["class:dialog frame.label"] = "bg:#ffffff #000000",
        ["class:dialog.body label"] = "#000000",
        ["class:dialog shadow"] = "bg:#000088",
        ["class:button"] = "",
        ["class:button.focused"] = "bg:#ff0000 #ffffff",
        ["class:checkbox"] = "",
        ["class:checkbox.focused"] = "noreverse bold",
        ["class:radio"] = "",
        ["class:radio.focused"] = "noreverse bold",
        ["class:radio-checked"] = "bold",
        ["class:radio-selected"] = "bold",
        ["class:validation-toolbar"] = "bg:#550000 #ffffff",
        ["class:control"] = "",
        ["class:frame"] = "",
        ["class:frame.border"] = "",
        ["class:frame.label"] = "#bbbbbb"
    };

    /// <summary>Returns a Style preloaded with the default rules.</summary>
    public static Style Create() => Style.FromDictionary(Defaults);
}
