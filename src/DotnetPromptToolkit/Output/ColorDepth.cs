namespace DotnetPromptToolkit.Output;

/// <summary>Mirrors prompt_toolkit.output.color_depth.ColorDepth.</summary>
public enum ColorDepth
{
    /// <summary>One color, monochrome.</summary>
    Depth1Bit,
    /// <summary>16 ANSI colors.</summary>
    Depth4Bit,
    /// <summary>256 colors.</summary>
    Depth8Bit,
    /// <summary>True color (24-bit).</summary>
    Depth24Bit
}

public static class ColorDepthExtensions
{
    public static ColorDepth FromEnvironment()
    {
        var term = Environment.GetEnvironmentVariable("COLORTERM") ?? string.Empty;
        if (term.Equals("truecolor", StringComparison.OrdinalIgnoreCase) ||
            term.Equals("24bit", StringComparison.OrdinalIgnoreCase))
        {
            return ColorDepth.Depth24Bit;
        }
        var termVar = Environment.GetEnvironmentVariable("TERM") ?? string.Empty;
        if (termVar.Contains("256", StringComparison.OrdinalIgnoreCase)) return ColorDepth.Depth8Bit;
        return ColorDepth.Depth4Bit;
    }
}
