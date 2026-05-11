using DotnetPromptToolkit.AutoSuggest;
using DotnetPromptToolkit.Buffers;
using DotnetPromptToolkit.Cache;
using DotnetPromptToolkit.Clipboard;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.Contrib.RegularLanguages;
using DotnetPromptToolkit.CursorShapes;
using DotnetPromptToolkit.DataStructures;
using DotnetPromptToolkit.Enums;
using DotnetPromptToolkit.Filters;
using DotnetPromptToolkit.FormattedText;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Input;
using DotnetPromptToolkit.KeyBinding;
using DotnetPromptToolkit.Keys;
using DotnetPromptToolkit.Layout;
using DotnetPromptToolkit.Lexers;
using DotnetPromptToolkit.MouseEvents;
using DotnetPromptToolkit.Output;
using DotnetPromptToolkit.PatchStdout;
using DotnetPromptToolkit.Search;
using DotnetPromptToolkit.Selection;
using DotnetPromptToolkit.Shortcuts;
using DotnetPromptToolkit.Styles;
using DotnetPromptToolkit.Validation;
using DotnetPromptToolkit.Widgets;
using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using PromptCompletion = DotnetPromptToolkit.Completion.Completion;
using PromptFormattedText = DotnetPromptToolkit.FormattedText.FormattedText;

namespace DotnetPromptToolkit.Tests;

public sealed class ExpandedPortTests
{
    [Fact]
    public void DataStructuresHaveExpectedShape()
    {
        Assert.Equal(new Point(1, 2), new Point(1, 2));
        Assert.Equal(new Size(3, 4), new Size(3, 4));
    }

    [Fact]
    public void EditingModeAndCursorShapesAreReadable()
    {
        Assert.Equal(EditingMode.Emacs, EditingMode.Emacs);
        var config = new StaticCursorShape(CursorShape.Beam);
        Assert.Equal(CursorShape.Beam, config.GetCursorShape());
        var dyn = new DynamicCursorShapeConfig(() => CursorShape.Block);
        Assert.Equal(CursorShape.Block, dyn.GetCursorShape());
    }

    [Fact]
    public void MouseEventCarriesPositionTypeAndModifier()
    {
        var ev = new MouseEvent(new Point(2, 3), MouseEventType.MouseDown, MouseButton.Left, MouseModifier.Shift);
        Assert.Equal(MouseButton.Left, ev.Button);
        Assert.True((ev.Modifiers & MouseModifier.Shift) != 0);
    }

    [Fact]
    public void SelectionStateRangeIsSorted()
    {
        var state = new SelectionState(originalCursorPosition: 7);
        var range = state.GetRange(2);
        Assert.Equal((2, 8), range);
    }

    [Fact]
    public void SearchStateForwardsAndBackwards()
    {
        var doc = new Document("hello world hello", 6);
        var fwd = IncrementalSearcher.Find(doc, new SearchState("hello"));
        Assert.Equal(12, fwd?.Index);
        var back = IncrementalSearcher.Find(doc, new SearchState("hello", SearchDirection.Backward), startIndex: 12);
        Assert.Equal(0, back?.Index);
    }

    [Fact]
    public void FilterAlgebraComposes()
    {
        Filter t = Filter.Always;
        Filter f = Filter.Never;
        Assert.True((t & t).Evaluate());
        Assert.False((t & f).Evaluate());
        Assert.True((t | f).Evaluate());
        Assert.False((~t).Evaluate());
        Assert.True((~f).Evaluate());
        var c = Filter.From(() => true);
        Assert.True(c.Evaluate());
    }

    [Fact]
    public async Task AutoSuggestFromHistoryReturnsTrailingText()
    {
        var history = new InMemoryHistory();
        history.Append("hello world");
        var suggest = new AutoSuggestFromHistory(history);
        var result = await suggest.GetSuggestionAsync(new Document("hello", 5));
        Assert.Equal(" world", result?.Text);

        var dummy = new DummyAutoSuggest();
        Assert.Null(await dummy.GetSuggestionAsync(new Document("x", 1)));
    }

    [Fact]
    public void SimpleLexerReturnsLine()
    {
        var lexer = new SimpleLexer("class:test");
        var fn = lexer.LexDocument(new Document("foo\nbar"));
        Assert.Equal("bar", fn(1).ToPlainText());
        Assert.Equal("class:test", fn(1).Fragments[0].Style);
    }

    [Fact]
    public void CacheStoresAndEvictsOldEntries()
    {
        var cache = new SimpleCache<string, int>(maxSize: 2);
        cache.Get("a", () => 1);
        cache.Get("b", () => 2);
        cache.Get("c", () => 3);
        Assert.False(cache.TryGet("a", out _));
        Assert.True(cache.TryGet("b", out var b));
        Assert.Equal(2, b);
    }

    [Fact]
    public void KeyNamesMatchPythonIdentifiers()
    {
        Assert.Equal("c-a", KeyNames.Name(DotnetPromptToolkit.Keys.Keys.ControlA));
        Assert.Equal("backspace", KeyNames.Name(DotnetPromptToolkit.Keys.Keys.Backspace));
        Assert.Equal("s-tab", KeyNames.Name(DotnetPromptToolkit.Keys.Keys.BackTab));
        Assert.Contains(DotnetPromptToolkit.Keys.Keys.F12, KeyNames.All);
    }

    [Fact]
    public async Task FuzzyAndMergedAndDeduplicateCompletersWork()
    {
        var doc = new Document("ab", 2);
        var fuzzy = new FuzzyWordCompleter(["abc", "axb", "zzz"]);
        var fuzzyResults = await fuzzy.GetCompletionsAsync(doc);
        Assert.Equal(new[] { "abc", "axb" }, fuzzyResults.Select(c => c.Text).ToArray());

        var merged = new MergedCompleter(new ICompleter[]
        {
            new WordCompleter(["alpha", "alphabet"]),
            new WordCompleter(["alphabet"])
        });
        var mergedDoc = new Document("al", 2);
        var mergedResults = await merged.GetCompletionsAsync(mergedDoc);
        Assert.Equal(3, mergedResults.Count);

        var deduped = new DeduplicateCompleter(merged);
        Assert.Equal(2, (await deduped.GetCompletionsAsync(mergedDoc)).Count);
    }

    [Fact]
    public async Task NestedCompleterRoutesToInnerCompleter()
    {
        var nested = NestedCompleter.FromNestedDict(new Dictionary<string, object?>
        {
            ["show"] = new Dictionary<string, object?>
            {
                ["users"] = null,
                ["roles"] = null
            },
            ["help"] = null
        });
        var topLevel = await nested.GetCompletionsAsync(new Document("sh", 2));
        Assert.Equal(new[] { "show" }, topLevel.Select(c => c.Text).ToArray());

        var second = await nested.GetCompletionsAsync(new Document("show ", 5));
        var second2 = await nested.GetCompletionsAsync(new Document("show u", 6));
        Assert.Contains("users", second.Select(c => c.Text));
        Assert.Equal(new[] { "users" }, second2.Select(c => c.Text).ToArray());
    }

    [Fact]
    public void CompletionStateNavigatesCircularly()
    {
        var state = new CompletionState(0, new[] { new PromptCompletion("a"), new PromptCompletion("b") });
        Assert.Equal("a", state.Current?.Text);
        state.GoToNext();
        Assert.Equal("b", state.Current?.Text);
        state.GoToNext();
        Assert.Equal("a", state.Current?.Text);
        state.GoToPrevious();
        Assert.Equal("b", state.Current?.Text);
    }

    [Fact]
    public void FileHistoryRoundTripsAcrossInstances()
    {
        var path = Path.Combine(Path.GetTempPath(), $"dpt-history-{Guid.NewGuid():N}");
        try
        {
            var first = new FileHistory(path);
            first.Append("line one");
            first.Append("line\ntwo");
            var second = new FileHistory(path);
            Assert.Equal(new[] { "line one", "line\ntwo" }, second.GetStrings().ToArray());
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void ColorDepthReadsFromEnvironmentSafely() =>
        Assert.True(Enum.IsDefined(typeof(ColorDepth), ColorDepthExtensions.FromEnvironment()));

    [Fact]
    public void Vt100OutputBuffersAnsiSequences()
    {
        var writer = new StringWriter();
        var output = new Vt100Output(writer);
        output.Write("hi");
        output.CursorGoto(2, 4);
        output.SetForeground(1);
        Assert.Equal("hi\u001b[3;5H\u001b[31m", output.DrainBuffer());
        output.WriteLine("done");
        output.Flush();
        Assert.Equal("done" + Environment.NewLine, writer.ToString());
    }

    [Fact]
    public void HtmlFormattedTextParsesBoldItalic()
    {
        var formatted = HtmlFormattedText.Parse("<b>hi</b> <i>there</i>");
        Assert.Equal("hi there", formatted.ToPlainText());
        Assert.Equal("class:b", formatted.Fragments[0].Style);
        Assert.Equal("class:i", formatted.Fragments[2].Style);
    }

    [Fact]
    public void AnsiFormattedTextParsesSgrColors()
    {
        var formatted = AnsiFormattedText.Parse("\u001b[31mred\u001b[0m plain");
        Assert.Equal("red plain", formatted.ToPlainText());
        Assert.Contains("fg:ansired", formatted.Fragments[0].Style);
    }

    [Fact]
    public void FormattedTextUtilsSplitLines()
    {
        var lines = FormattedTextUtils.SplitLines(new[]
        {
            new FormattedTextFragment("class:a", "hello\nworld"),
            new FormattedTextFragment("class:b", "!")
        });
        Assert.Equal(2, lines.Count);
        Assert.Equal("hello", lines[0][0].Text);
        Assert.Equal("world", lines[1][0].Text);
    }

    [Fact]
    public void NamedColorsResolveBuiltInsAndHex()
    {
        Assert.Equal("#ff0000", NamedColors.Resolve("red"));
        Assert.Equal("#abcdef", NamedColors.Resolve("#abcdef"));
        Assert.Null(NamedColors.Resolve("not-a-color"));
    }

    [Fact]
    public void StyleTransformationsCompose()
    {
        IStyleTransformation transform = new MergedStyleTransformation(new IStyleTransformation[]
        {
            new SwapLightAndDarkStyleTransformation(),
            new IdentityStyleTransformation()
        });
        Assert.Equal("fg:white", transform.Transform("fg:black"));
        Assert.Equal("fg:black", transform.Transform("fg:white"));
    }

    [Fact]
    public void DefaultStyleProvidesCommonClasses()
    {
        var style = DefaultStyle.Create();
        Assert.Equal("ansiblue", style.GetStyleForToken("class:control-character"));
    }

    [Fact]
    public void DimensionDistributorMatchesWeights()
    {
        var sizes = DimensionDistributor.Distribute(new[]
        {
            new Dimension(min: 2, weight: 1),
            new Dimension(min: 0, weight: 3)
        }, total: 10);
        Assert.Equal(10, sizes.Sum());
        Assert.True(sizes[0] >= 2);
        Assert.True(sizes[1] >= 0);
    }

    [Fact]
    public void WindowControlsAndFloatContainerRender()
    {
        var window = new Window(new FormattedTextControl(new[] { new FormattedTextFragment("", "hello") }));
        Assert.Equal("hello", window.Render().ToPlainText());

        var floats = new FloatContainer(new TextLabel("body"), new[] { new Float(new TextLabel("popup")) });
        var rendered = floats.Render().ToPlainText();
        Assert.Contains("body", rendered);
        Assert.Contains("popup", rendered);
    }

    [Fact]
    public void NumberedAndScrollbarMarginsRender()
    {
        var numbered = new NumberedMargin().CreateMargin(2).ToPlainText();
        Assert.Contains("1", numbered);
        Assert.Contains("2", numbered);
        var scrollbar = new ScrollbarMargin(contentHeight: 100, viewHeight: 10, viewTop: 0).CreateMargin(10).ToPlainText();
        Assert.Equal(10, scrollbar.Length);
    }

    [Fact]
    public void HighlightSearchProcessorMarksMatches()
    {
        var input = new PromptFormattedText().Append("foo bar foo");
        var processed = new HighlightSearchProcessor("foo").Process(input);
        Assert.Equal("foo bar foo", processed.ToPlainText());
        Assert.Contains(processed.Fragments, f => f.Style == "class:search");
    }

    [Fact]
    public void PasswordProcessorMasksAllCharacters()
    {
        var input = new PromptFormattedText().Append("secret");
        Assert.Equal("******", new PasswordProcessor().Process(input).ToPlainText());
    }

    [Fact]
    public void CompletionsMenuHighlightsSelected()
    {
        var menu = new CompletionsMenu(new[] { "alpha", "beta" }, selectedIndex: 1).Render();
        Assert.Contains(menu.Fragments, f => f.Style == "class:completion-menu.completion.current" && f.Text.Contains("beta"));
    }

    [Fact]
    public void FrameAndBoxAndCheckboxAndRadioListRender()
    {
        var frame = new Frame(new TextLabel("body"), "title").Render().ToPlainText();
        Assert.Contains("title", frame);
        Assert.Contains("body", frame);

        Assert.Contains("body", new Box(new TextLabel("body"), padding: 1).Render().ToPlainText());

        var checkbox = new Checkbox("done");
        Assert.Contains("[ ]", checkbox.Render().ToPlainText());
        checkbox.Toggle();
        Assert.Contains("[x]", checkbox.Render().ToPlainText());

        var radio = new RadioList<string>(new[] { ("a", "Alpha"), ("b", "Beta") });
        radio.Next();
        Assert.Equal("b", radio.Current);
        radio.Previous();
        Assert.Equal("a", radio.Current);
    }

    [Fact]
    public void DialogsHelperRendersDialog()
    {
        var dialog = Dialogs.MessageDialog("Hello", "World");
        var rendered = dialog.Render().ToPlainText();
        Assert.Contains("Hello", rendered);
        Assert.Contains("World", rendered);
        Assert.Contains("OK", rendered);
    }

    [Fact]
    public void ProgressBarTracksPercent()
    {
        var bar = new ProgressBar(total: 4);
        bar.Advance(2);
        var rendered = bar.Render();
        Assert.Contains("2/4", rendered);
    }

    [Fact]
    public void EmacsBindingsHandleControlAandU()
    {
        var bindings = EmacsBindings.Create();
        var processor = new KeyProcessor(bindings);
        var buffer = new PromptBuffer("hello world");
        buffer.CursorRight(11);
        processor.Feed(buffer, new KeyPress(Key.ControlA));
        Assert.Equal(0, buffer.CursorPosition);
        processor.Feed(buffer, new KeyPress(Key.ControlE));
        Assert.Equal(11, buffer.CursorPosition);
    }

    [Fact]
    public void GrammarCompilerMatchesAndExtractsVariables()
    {
        var grammar = GrammarCompiler.Compile(@"(?<command>\w+)\s+(?<argument>\w+)");
        var vars = grammar.Variables("show users");
        Assert.Equal("show", vars["command"]);
        Assert.Equal("users", vars["argument"]);
        Assert.NotNull(grammar.Match("show users"));
    }

    [Fact]
    public void PatchStdoutBuffersAndFlushes()
    {
        var saved = Console.Out;
        var capture = new StringWriter();
        Console.SetOut(capture);
        try
        {
            using (var scope = new PatchStdoutScope())
            {
                Console.WriteLine("buffered");
                Assert.Equal("", capture.ToString());
            }
            Assert.Contains("buffered", capture.ToString());
        }
        finally
        {
            Console.SetOut(saved);
        }
    }

    [Fact]
    public void KillRingClipboardRotates()
    {
        var clip = new KillRingClipboard();
        clip.SetData(new ClipboardData("a"));
        clip.SetData(new ClipboardData("b"));
        Assert.Equal("b", clip.GetData().Text);
        clip.Rotate();
        Assert.Equal("a", clip.GetData().Text);
    }

    [Fact]
    public void WordValidatorAndRegexValidator()
    {
        var word = new WordValidator(new[] { "yes", "no" });
        word.Validate(new Document("yes"));
        Assert.Throws<ValidationError>(() => word.Validate(new Document("maybe")));

        var regex = new RegexValidator(new System.Text.RegularExpressions.Regex(@"^\d+$"));
        regex.Validate(new Document("123"));
        Assert.Throws<ValidationError>(() => regex.Validate(new Document("abc")));
    }

    [Fact]
    public void AnsiInputParserHandlesExtendedSequences()
    {
        var keys = new AnsiInputParser().Feed("\u001b[H\u001b[F\u001b[3~\u001b[5~\u001bOC\u0001").ToArray();
        Assert.Equal(Key.Home, keys[0].Key);
        Assert.Equal(Key.End, keys[1].Key);
        Assert.Equal(Key.Delete, keys[2].Key);
        Assert.Equal(Key.PageUp, keys[3].Key);
        Assert.Equal(Key.Right, keys[4].Key);
        Assert.Equal(Key.ControlA, keys[5].Key);
    }
}
