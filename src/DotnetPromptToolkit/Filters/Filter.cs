namespace DotnetPromptToolkit.Filters;

/// <summary>
/// Mirrors prompt_toolkit.filters.Filter. A composable boolean predicate that can be
/// combined with operators &amp; (AND), | (OR), and ~ (NOT).
/// </summary>
public abstract class Filter
{
    public abstract bool Evaluate();

    public static Filter operator &(Filter a, Filter b) => new AndFilter(a, b);
    public static Filter operator |(Filter a, Filter b) => new OrFilter(a, b);
    public static Filter operator ~(Filter f) => new NotFilter(f);
    public static bool operator true(Filter f) => f.Evaluate();
    public static bool operator false(Filter f) => !f.Evaluate();

    public static implicit operator bool(Filter f) => f.Evaluate();

    public static Filter Always { get; } = new AlwaysFilter();
    public static Filter Never { get; } = new NeverFilter();

    public static Filter From(Func<bool> condition) => new Condition(condition);
    public static Filter From(bool value) => value ? Always : Never;
}

internal sealed class AlwaysFilter : Filter
{
    public override bool Evaluate() => true;
}

internal sealed class NeverFilter : Filter
{
    public override bool Evaluate() => false;
}

public sealed class Condition(Func<bool> predicate) : Filter
{
    public override bool Evaluate() => predicate();
}

internal sealed class AndFilter(Filter a, Filter b) : Filter
{
    public override bool Evaluate() => a.Evaluate() && b.Evaluate();
}

internal sealed class OrFilter(Filter a, Filter b) : Filter
{
    public override bool Evaluate() => a.Evaluate() || b.Evaluate();
}

internal sealed class NotFilter(Filter inner) : Filter
{
    public override bool Evaluate() => !inner.Evaluate();
}
