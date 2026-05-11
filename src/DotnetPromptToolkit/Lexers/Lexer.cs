using DotnetPromptToolkit.Buffers;
using DotnetPromptToolkit.FormattedText;

namespace DotnetPromptToolkit.Lexers;

/// <summary>Mirrors prompt_toolkit.lexers.Lexer.</summary>
public interface ILexer
{
    Func<int, FormattedText.FormattedText> LexDocument(Document document);
}

/// <summary>Lexer that returns the line as-is. Mirrors prompt_toolkit.lexers.SimpleLexer.</summary>
public sealed class SimpleLexer(string style = "") : ILexer
{
    public Func<int, FormattedText.FormattedText> LexDocument(Document document)
    {
        var lines = document.Lines;
        return lineNumber =>
        {
            if (lineNumber < 0 || lineNumber >= lines.Count) return new FormattedText.FormattedText();
            return new FormattedText.FormattedText().Append(lines[lineNumber], style);
        };
    }
}

/// <summary>Wraps a per-line tokenizer. Mirrors PygmentsLexer in concept.</summary>
public sealed class DelegatingLexer(Func<string, IReadOnlyList<(string Style, string Text)>> tokenize) : ILexer
{
    public Func<int, FormattedText.FormattedText> LexDocument(Document document)
    {
        var lines = document.Lines;
        return lineNumber =>
        {
            var formatted = new FormattedText.FormattedText();
            if (lineNumber < 0 || lineNumber >= lines.Count) return formatted;
            foreach (var (style, text) in tokenize(lines[lineNumber]))
            {
                formatted.Append(text, style);
            }
            return formatted;
        };
    }
}
