namespace DotnetPromptToolkit.Styles;

/// <summary>
/// Mirrors prompt_toolkit.styles.style_transformation.StyleTransformation.
/// </summary>
public interface IStyleTransformation
{
    string Transform(string style);
}

public sealed class IdentityStyleTransformation : IStyleTransformation
{
    public string Transform(string style) => style;
}

/// <summary>Mirrors prompt_toolkit.styles.style_transformation.SwapLightAndDarkStyleTransformation.</summary>
public sealed class SwapLightAndDarkStyleTransformation : IStyleTransformation
{
    public string Transform(string style)
    {
        if (string.IsNullOrEmpty(style)) return style;
        return style
            .Replace("fg:white", "__SWAP__")
            .Replace("fg:black", "fg:white")
            .Replace("__SWAP__", "fg:black");
    }
}

/// <summary>Mirrors prompt_toolkit.styles.style_transformation.ConditionalStyleTransformation.</summary>
public sealed class ConditionalStyleTransformation(IStyleTransformation inner, Func<bool> filter) : IStyleTransformation
{
    public string Transform(string style) => filter() ? inner.Transform(style) : style;
}

/// <summary>Composes multiple transformations.</summary>
public sealed class MergedStyleTransformation(IEnumerable<IStyleTransformation> transformations) : IStyleTransformation
{
    private readonly IStyleTransformation[] _transformations = transformations.ToArray();
    public string Transform(string style)
    {
        foreach (var transformation in _transformations) style = transformation.Transform(style);
        return style;
    }
}
