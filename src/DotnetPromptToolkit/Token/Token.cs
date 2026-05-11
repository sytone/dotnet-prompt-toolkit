namespace DotnetPromptToolkit.Token;

/// <summary>Mirrors prompt_toolkit.token. A token is just a style name in the new API.</summary>
public sealed record Token(string Name)
{
    public static Token Text { get; } = new("Token.Text");
    public static Token Generic { get; } = new("Token.Generic");
    public static Token Comment { get; } = new("Token.Comment");
    public static Token Keyword { get; } = new("Token.Keyword");
    public static Token NameToken { get; } = new("Token.Name");
    public static Token String { get; } = new("Token.String");
    public static Token Number { get; } = new("Token.Number");
    public static Token Operator { get; } = new("Token.Operator");
    public static Token Punctuation { get; } = new("Token.Punctuation");
    public static Token Error { get; } = new("Token.Error");

    public override string ToString() => Name;
}
