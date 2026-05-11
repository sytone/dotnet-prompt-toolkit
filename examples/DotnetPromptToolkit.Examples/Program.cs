using System.Text.Json;
using DotnetPromptToolkit.Application;
using DotnetPromptToolkit.AutoSuggest;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.Contrib.RegularLanguages;
using DotnetPromptToolkit.FormattedText;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Layout;
using DotnetPromptToolkit.Search;
using DotnetPromptToolkit.Shortcuts;
using DotnetPromptToolkit.Styles;
using DotnetPromptToolkit.Widgets;
using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using PromptDocument = DotnetPromptToolkit.Buffers.Document;

var example = args.FirstOrDefault() ?? "basic";

switch (example)
{
    case "completion":
        await CompletionExample();
        break;
    case "layout":
        LayoutExample();
        break;
    case "parity":
        ParityScenario(args.Skip(1).FirstOrDefault() ?? "all");
        break;
    default:
        await BasicPromptExample();
        break;
}

static async Task BasicPromptExample()
{
    var session = new PromptSession();
    var name = await session.PromptAsync("Name: ");
    Console.WriteLine($"Hello {name}");
}

static async Task CompletionExample()
{
    var session = new PromptSession(completer: new WordCompleter(["red", "green", "blue"]));
    var value = await session.PromptAsync("Color: ");
    Console.WriteLine($"Selected {value}");
}

static void LayoutExample()
{
    var layout = new HSplit(new Label("dotnet-prompt-toolkit"), new TextArea(prompt: "> "));
    Console.WriteLine(layout.Render().ToPlainText());
}

static void ParityScenario(string scenario)
{
    var result = scenario switch
    {
        "document" => DocumentScenario(),
        "completion" => CompletionScenario(),
        "fuzzy-completion" => FuzzyCompletionScenario(),
        "nested-completion" => NestedCompletionScenario(),
        "history" => HistoryScenario(),
        "formatted-text" => FormattedTextScenario(),
        "html" => HtmlFormattedTextScenario(),
        "ansi" => AnsiFormattedTextScenario(),
        "layout" => LayoutScenario(),
        "search" => SearchScenario(),
        "auto-suggest" => AutoSuggestScenario(),
        "named-colors" => NamedColorsScenario(),
        "grammar" => GrammarScenario(),
        "progress-bar" => ProgressBarScenario(),
        "frame" => FrameScenario(),
        "all" => new
        {
            scenario = "all",
            results = new object[]
            {
                DocumentScenario(),
                CompletionScenario(),
                FuzzyCompletionScenario(),
                NestedCompletionScenario(),
                HistoryScenario(),
                FormattedTextScenario(),
                HtmlFormattedTextScenario(),
                AnsiFormattedTextScenario(),
                LayoutScenario(),
                SearchScenario(),
                AutoSuggestScenario(),
                NamedColorsScenario(),
                GrammarScenario(),
                ProgressBarScenario(),
                FrameScenario()
            }
        },
        _ => throw new ArgumentException($"Unknown parity scenario '{scenario}'.", nameof(scenario))
    };

    Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
}

static object DocumentScenario()
{
    var document = new PromptDocument("alpha\nbeta gamma", 12);
    var position = document.TranslateIndexToPosition(document.CursorPosition);
    return new
    {
        scenario = "document",
        text = document.Text,
        cursorPosition = document.CursorPosition,
        row = position.Row,
        column = position.Column,
        currentLineBeforeCursor = document.CurrentLineBeforeCursor,
        wordBeforeCursor = document.GetWordBeforeCursor()
    };
}

static object CompletionScenario()
{
    var document = new PromptDocument("he", 2);
    var completer = new WordCompleter(["help", "hello", "world"]);
    var completions = completer.GetCompletionsAsync(document).AsTask().GetAwaiter().GetResult()
        .OrderBy(completion => completion.Text, StringComparer.Ordinal)
        .Select(completion => new
        {
            completion.Text,
            completion.StartPosition,
            Applied = completion.Apply(document)
        })
        .ToArray();
    return new { scenario = "completion", completions };
}

static object FuzzyCompletionScenario()
{
    var document = new PromptDocument("ab", 2);
    var completer = new FuzzyWordCompleter(["abc", "axb", "zzz"]);
    var completions = completer.GetCompletionsAsync(document).AsTask().GetAwaiter().GetResult()
        .Select(c => c.Text)
        .ToArray();
    return new { scenario = "fuzzy-completion", completions };
}

static object NestedCompletionScenario()
{
    var nested = NestedCompleter.FromNestedDict(new Dictionary<string, object?>
    {
        ["show"] = new Dictionary<string, object?> { ["users"] = null, ["roles"] = null },
        ["help"] = null
    });
    var document = new PromptDocument("show u", 6);
    var completions = nested.GetCompletionsAsync(document).AsTask().GetAwaiter().GetResult()
        .Select(c => c.Text)
        .OrderBy(x => x, StringComparer.Ordinal)
        .ToArray();
    return new { scenario = "nested-completion", completions };
}

static object HistoryScenario()
{
    var history = new InMemoryHistory();
    var buffer = new PromptBuffer(history: history);
    buffer.InsertText("abc");
    buffer.Accept();
    return new { scenario = "history", entries = history.GetStrings().ToArray() };
}

static object FormattedTextScenario()
{
    var formatted = new FormattedText()
        .Append("hello", "class:greeting")
        .Append(" world");
    return new
    {
        scenario = "formatted-text",
        plainText = formatted.ToPlainText(),
        fragments = formatted.Fragments.Select(f => new { f.Style, f.Text }).ToArray()
    };
}

static object HtmlFormattedTextScenario()
{
    var formatted = HtmlFormattedText.Parse("<b>bold</b> and <i>italic</i>");
    return new
    {
        scenario = "html",
        plainText = formatted.ToPlainText(),
        fragments = formatted.Fragments.Select(f => new { f.Style, f.Text }).ToArray()
    };
}

static object AnsiFormattedTextScenario()
{
    var formatted = AnsiFormattedText.Parse("\u001b[31mred\u001b[0m plain");
    return new
    {
        scenario = "ansi",
        plainText = formatted.ToPlainText()
    };
}

static object LayoutScenario()
{
    var layout = new HSplit(new Label("dotnet-prompt-toolkit"), new TextArea(prompt: "> "));
    return new { scenario = "layout", plainText = layout.Render().ToPlainText() };
}

static object SearchScenario()
{
    var document = new PromptDocument("hello world hello", 6);
    var forward = IncrementalSearcher.Find(document, new SearchState("hello"));
    var backward = IncrementalSearcher.Find(document, new SearchState("hello", SearchDirection.Backward), startIndex: 12);
    return new
    {
        scenario = "search",
        forwardIndex = forward?.Index,
        forwardLength = forward?.Length,
        backwardIndex = backward?.Index,
        backwardLength = backward?.Length
    };
}

static object AutoSuggestScenario()
{
    var history = new InMemoryHistory();
    history.Append("hello world");
    var suggest = new AutoSuggestFromHistory(history);
    var suggestion = suggest.GetSuggestionAsync(new PromptDocument("hello", 5)).AsTask().GetAwaiter().GetResult();
    return new { scenario = "auto-suggest", text = suggestion?.Text };
}

static object NamedColorsScenario() => new
{
    scenario = "named-colors",
    red = NamedColors.Resolve("red"),
    blue = NamedColors.Resolve("blue"),
    custom = NamedColors.Resolve("#abcdef")
};

static object GrammarScenario()
{
    var grammar = GrammarCompiler.Compile(@"(?<command>\w+)\s+(?<argument>\w+)");
    var vars = grammar.Variables("show users");
    return new
    {
        scenario = "grammar",
        command = vars["command"],
        argument = vars["argument"]
    };
}

static object ProgressBarScenario()
{
    var bar = new ProgressBar(total: 4, width: 8);
    bar.Advance(2);
    return new { scenario = "progress-bar", rendered = bar.Render() };
}

static object FrameScenario()
{
    var frame = new Frame(new TextLabel("body"), "title").Render().ToPlainText();
    return new { scenario = "frame", rendered = frame };
}
