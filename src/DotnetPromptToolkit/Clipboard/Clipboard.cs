namespace DotnetPromptToolkit.Clipboard;

public interface IClipboard
{
    string Text { get; set; }
}

public sealed class InMemoryClipboard : IClipboard
{
    public string Text { get; set; } = string.Empty;
}
