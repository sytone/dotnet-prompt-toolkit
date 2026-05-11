using DotnetPromptToolkit.Buffers;

namespace DotnetPromptToolkit.Validation;

/// <summary>Mirrors prompt_toolkit.validation.DummyValidator.</summary>
public sealed class DummyValidator : IValidator
{
    public void Validate(Document document) { }
}

/// <summary>Mirrors prompt_toolkit.validation.ThreadedValidator.</summary>
public sealed class ThreadedValidator(IValidator inner) : IValidator
{
    public void Validate(Document document) => Task.Run(() => inner.Validate(document)).GetAwaiter().GetResult();
}

/// <summary>Validates input against a list of allowed values.</summary>
public sealed class WordValidator(IEnumerable<string> allowed, bool ignoreCase = false, string errorMessage = "Invalid value") : IValidator
{
    private readonly HashSet<string> _allowed = new(allowed, ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    public void Validate(Document document)
    {
        if (!_allowed.Contains(document.Text)) throw new ValidationError(errorMessage, document.CursorPosition);
    }
}

/// <summary>Mirrors a regex-based validator.</summary>
public sealed class RegexValidator(System.Text.RegularExpressions.Regex pattern, string errorMessage = "Invalid value") : IValidator
{
    public void Validate(Document document)
    {
        if (!pattern.IsMatch(document.Text)) throw new ValidationError(errorMessage, document.CursorPosition);
    }
}
