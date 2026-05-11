namespace DotnetPromptToolkit.Layout;

/// <summary>
/// Mirrors prompt_toolkit.layout.dimension.Dimension. Describes minimum, max,
/// preferred and weight values for a layout slot.
/// </summary>
public sealed record Dimension
{
    public Dimension(int min = 0, int? max = null, int? preferred = null, int weight = 1)
    {
        if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
        if (max is { } m && m < min) throw new ArgumentOutOfRangeException(nameof(max));
        Min = min;
        Max = max ?? int.MaxValue;
        Preferred = preferred ?? min;
        Weight = weight;
    }

    public int Min { get; }
    public int Max { get; }
    public int Preferred { get; }
    public int Weight { get; }

    public static Dimension Exact(int amount) => new(amount, amount, amount);
    public static Dimension Zero() => new(0, 0, 0);
    public static Dimension Flexible(int weight = 1) => new(weight: weight);
}

/// <summary>Helpers mirroring prompt_toolkit.layout.dimension.D shortcuts.</summary>
public static class D
{
    public static Dimension Exact(int amount) => Dimension.Exact(amount);
    public static Dimension Min(int min) => new(min: min);
    public static Dimension Max(int max) => new(min: 0, max: max);
    public static Dimension Preferred(int preferred) => new(preferred: preferred);
}

/// <summary>
/// Computes how to distribute a total length to a list of dimensions, mirroring
/// prompt_toolkit.layout.dimension.sum_layout_dimensions / split_into_buckets.
/// </summary>
public static class DimensionDistributor
{
    public static int[] Distribute(IReadOnlyList<Dimension> dimensions, int total)
    {
        var n = dimensions.Count;
        if (n == 0) return Array.Empty<int>();
        var sizes = new int[n];
        var remaining = total;
        for (var i = 0; i < n; i++)
        {
            sizes[i] = Math.Min(dimensions[i].Min, remaining);
            remaining -= sizes[i];
        }
        // distribute towards preferred
        for (var i = 0; i < n && remaining > 0; i++)
        {
            var grow = Math.Min(remaining, dimensions[i].Preferred - sizes[i]);
            if (grow > 0) { sizes[i] += grow; remaining -= grow; }
        }
        // distribute remaining proportionally to weights up to max
        if (remaining > 0)
        {
            var weightSum = dimensions.Sum(d => d.Weight);
            if (weightSum > 0)
            {
                for (var i = 0; i < n && remaining > 0; i++)
                {
                    var portion = (int)((double)dimensions[i].Weight / weightSum * remaining);
                    var grow = Math.Max(0, Math.Min(portion, dimensions[i].Max - sizes[i]));
                    sizes[i] += grow;
                    remaining -= grow;
                }
                while (remaining > 0)
                {
                    var grew = false;
                    for (var i = 0; i < n && remaining > 0; i++)
                    {
                        if (sizes[i] < dimensions[i].Max) { sizes[i]++; remaining--; grew = true; }
                    }
                    if (!grew) break;
                }
            }
        }
        return sizes;
    }
}
