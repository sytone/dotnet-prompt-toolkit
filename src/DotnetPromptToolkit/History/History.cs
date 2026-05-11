namespace DotnetPromptToolkit.History;

public interface IHistory
{
    void Append(string value);
    IReadOnlyList<string> GetStrings();
}

public sealed class InMemoryHistory : IHistory
{
    private readonly List<string> _items = [];
    public void Append(string value)
    {
        if (!string.IsNullOrEmpty(value)) _items.Add(value);
    }
    public IReadOnlyList<string> GetStrings() => _items;
}
