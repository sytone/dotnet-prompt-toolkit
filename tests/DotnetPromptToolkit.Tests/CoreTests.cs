using DotnetPromptToolkit.Buffers;
using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using PromptFormattedText = DotnetPromptToolkit.FormattedText.FormattedText;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.FormattedText;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Input;
using DotnetPromptToolkit.KeyBinding;
using DotnetPromptToolkit.Layout;
using DotnetPromptToolkit.Rendering;
using DotnetPromptToolkit.Search;
using DotnetPromptToolkit.Styles;
using DotnetPromptToolkit.Validation;

namespace DotnetPromptToolkit.Tests;

public sealed class CoreTests
{
    [Fact]
    public void DocumentTracksCursorLinesAndWords()
    {
        var document = new Document("alpha\nbeta gamma", 12);
        Assert.Equal(new CursorPosition(1, 6), document.TranslateIndexToPosition(12));
        Assert.Equal(12, document.TranslatePositionToIndex(1, 6));
        Assert.Equal("beta g", document.CurrentLineBeforeCursor);
        Assert.Equal("g", document.GetWordBeforeCursor());
    }

    [Fact]
    public void BufferEditsUndoValidatesAndStoresHistory()
    {
        var history = new InMemoryHistory();
        var buffer = new PromptBuffer(history: history, validator: new DelegateValidator(d =>
        {
            if (d.Text.Length < 3) throw new ValidationError("too short", d.CursorPosition);
        }));

        buffer.InsertText("abc");
        buffer.DeleteBeforeCursor();
        Assert.Equal("ab", buffer.Text);
        Assert.True(buffer.Undo());
        Assert.Equal("abc", buffer.Text);
        buffer.Accept();
        Assert.Equal(["abc"], history.GetStrings());
    }

    [Fact]
    public async Task WordCompleterReplacesWordBeforeCursor()
    {
        var buffer = new PromptBuffer("he", completer: new WordCompleter(["help", "hello", "world"]));
        var completions = await buffer.CompleteAsync();
        Assert.Equal(["hello", "help"], completions.Select(c => c.Text).Order().ToArray());
        Assert.Equal("hello", completions.Single(c => c.Text == "hello").Apply(buffer.Document));
    }

    [Fact]
    public void KeyProcessorHandlesDefaultEditingAndCustomBindings()
    {
        var bindings = new KeyBindings().Add(Key.ControlD, (buffer, _) => buffer.InsertText("done"));
        var processor = new KeyProcessor(bindings);
        var buffer = new PromptBuffer();
        processor.Feed(buffer, new KeyPress(Key.Character, 'a'));
        processor.Feed(buffer, new KeyPress(Key.Character, 'b'));
        processor.Feed(buffer, new KeyPress(Key.Backspace));
        processor.Feed(buffer, new KeyPress(Key.ControlD));
        Assert.Equal("adone", buffer.Text);
    }

    [Fact]
    public void AnsiParserMapsCommonTerminalSequences()
    {
        var keys = new AnsiInputParser().Feed("a\u001b[D\u007f\r").ToArray();
        Assert.Equal(Key.Character, keys[0].Key);
        Assert.Equal(Key.Left, keys[1].Key);
        Assert.Equal(Key.Backspace, keys[2].Key);
        Assert.Equal(Key.Enter, keys[3].Key);
    }

    [Fact]
    public void RenderingLayoutAndStylesProducePlainTextAndDiffs()
    {
        var layout = new HSplit(new Label("Title"), new TextArea(new PromptBuffer("body"), "> "));
        var text = layout.Render();
        Assert.Equal("Title" + Environment.NewLine + "> body", text.ToPlainText());

        var style = Style.FromDictionary(new Dictionary<string, string> { ["class:title"] = "bold" });
        Assert.Equal("bold", style.GetStyleForToken("class:title"));

        var renderer = new Renderer();
        Assert.NotEmpty(renderer.Render(text));
        Assert.Empty(renderer.Render(text));
    }

    [Fact]
    public void ScreenSearchAndFormattedTextWork()
    {
        var formatted = new PromptFormattedText().Append("hello", "class:greeting").Append(" world");
        Assert.Equal("hello world", formatted.ToPlainText());

        var screen = new Screen();
        screen.Write(1, 2, "abc");
        Assert.Equal("  abc", screen.GetLine(1));

        Assert.Equal(new SearchResult(6, 5), Searcher.FindNext(new Document("hello world", 0), "world"));
    }
}
