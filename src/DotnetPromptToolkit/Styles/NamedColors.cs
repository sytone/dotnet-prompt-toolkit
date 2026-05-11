namespace DotnetPromptToolkit.Styles;

/// <summary>
/// Mirrors a subset of prompt_toolkit.styles.named_colors. Only the most common
/// CSS named colors are listed; the dictionary is used when resolving color
/// names in styles.
/// </summary>
public static class NamedColors
{
    public static IReadOnlyDictionary<string, string> Colors { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["aliceblue"] = "#f0f8ff",
        ["antiquewhite"] = "#faebd7",
        ["aqua"] = "#00ffff",
        ["aquamarine"] = "#7fffd4",
        ["azure"] = "#f0ffff",
        ["beige"] = "#f5f5dc",
        ["black"] = "#000000",
        ["blue"] = "#0000ff",
        ["brown"] = "#a52a2a",
        ["chocolate"] = "#d2691e",
        ["coral"] = "#ff7f50",
        ["cornflowerblue"] = "#6495ed",
        ["crimson"] = "#dc143c",
        ["cyan"] = "#00ffff",
        ["darkblue"] = "#00008b",
        ["darkcyan"] = "#008b8b",
        ["darkgray"] = "#a9a9a9",
        ["darkgreen"] = "#006400",
        ["darkmagenta"] = "#8b008b",
        ["darkorange"] = "#ff8c00",
        ["darkred"] = "#8b0000",
        ["fuchsia"] = "#ff00ff",
        ["gold"] = "#ffd700",
        ["gray"] = "#808080",
        ["green"] = "#008000",
        ["indigo"] = "#4b0082",
        ["lavender"] = "#e6e6fa",
        ["lightblue"] = "#add8e6",
        ["lime"] = "#00ff00",
        ["magenta"] = "#ff00ff",
        ["maroon"] = "#800000",
        ["navy"] = "#000080",
        ["olive"] = "#808000",
        ["orange"] = "#ffa500",
        ["orchid"] = "#da70d6",
        ["pink"] = "#ffc0cb",
        ["purple"] = "#800080",
        ["red"] = "#ff0000",
        ["silver"] = "#c0c0c0",
        ["teal"] = "#008080",
        ["tomato"] = "#ff6347",
        ["violet"] = "#ee82ee",
        ["white"] = "#ffffff",
        ["yellow"] = "#ffff00"
    };

    public static string? Resolve(string nameOrHex) =>
        Colors.TryGetValue(nameOrHex, out var hex) ? hex : (nameOrHex.StartsWith('#') ? nameOrHex : null);
}
