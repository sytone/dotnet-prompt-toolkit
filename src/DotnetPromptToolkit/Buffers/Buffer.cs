using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Validation;

namespace DotnetPromptToolkit.Buffers;

public sealed class Buffer
{
    private readonly Stack<Document> _undo = new();

    public Buffer(string text = "", IHistory? history = null, ICompleter? completer = null, IValidator? validator = null)
    {
        Document = new Document(text);
        History = history ?? new InMemoryHistory();
        Completer = completer;
        Validator = validator;
    }

    public Document Document { get; private set; }
    public IHistory History { get; }
    public ICompleter? Completer { get; set; }
    public IValidator? Validator { get; set; }
    public string Text => Document.Text;
    public int CursorPosition => Document.CursorPosition;

    public void Reset(string text = "") => SetDocument(new Document(text), trackUndo: false);
    public void InsertText(string text) => SetDocument(Document.InsertBeforeCursor(text));
    public void DeleteBeforeCursor(int count = 1) => SetDocument(Document.DeleteBeforeCursor(count));
    public void CursorLeft(int count = 1) => SetDocument(Document.MoveCursor(-count), trackUndo: false);
    public void CursorRight(int count = 1) => SetDocument(Document.MoveCursor(count), trackUndo: false);

    public bool Undo()
    {
        if (_undo.Count == 0) return false;
        Document = _undo.Pop();
        return true;
    }

    public void Validate() => Validator?.Validate(Document);

    public async ValueTask<IReadOnlyList<Completion.Completion>> CompleteAsync(CancellationToken cancellationToken = default) =>
        Completer is null ? [] : await Completer.GetCompletionsAsync(Document, cancellationToken).ConfigureAwait(false);

    public void Accept()
    {
        Validate();
        History.Append(Text);
    }

    private void SetDocument(Document document, bool trackUndo = true)
    {
        if (trackUndo) _undo.Push(Document);
        Document = document;
    }
}
