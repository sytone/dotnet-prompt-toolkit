using DotnetPromptToolkit.Buffers;

namespace DotnetPromptToolkit.Validation;

public sealed class ValidationError(string message, int cursorPosition = 0) : Exception(message)
{
    public int CursorPosition { get; } = cursorPosition;
}

public interface IValidator
{
    void Validate(Document document);
}

public sealed class DelegateValidator(Action<Document> validate) : IValidator
{
    public void Validate(Document document) => validate(document);
}
